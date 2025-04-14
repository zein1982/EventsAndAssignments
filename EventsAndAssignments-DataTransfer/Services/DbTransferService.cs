using System.Data;
using System.Data.Common;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Dapper;
using EventsAndAssignments_DataTransfer.DAO;
using EventsAndAssignments_DataTransfer.DAO.MIMPublish2Db;
using EventsAndAssignments_DataTransfer.DTO;
using EventsAndAssignments_DataTransfer.Interfaces;

namespace EventsAndAssignments_DataTransfer.Services
{
    /// <summary>
    /// Сервис обмена (репликации) данных между базами данных
    /// </summary>
    public class DbTransferService : BackgroundService
    {
        private readonly List<TimeSpan>                 _serviceStartTimeList;
        private readonly List<IServiceEvent>            _serviceEventList = new();
        private readonly ILogger<DbTransferService>     _logger;
        private readonly MIMPublish2Context             _mimPublish2Context;
        private readonly EventsAndAssignmentsContext    _eventsAndAssingmentsContext;
        private readonly bool                           _deleteOldEmployees;
        private readonly bool                           _deleteOldOrganizations;
        private bool                                    _serviceOnPause;
        private readonly DbConnection                   _mimConnection;
        private readonly DbConnection                   _easConnection;

        public DbTransferService(DbTransferServiceControl serviceControl, IServiceProvider serviceProvider,
            ConnectionStringService connStrService, IConfiguration configuration, ILogger<DbTransferService> logger)
        {
            serviceControl.GetServiceActivityStatusDelegate =
                new DbTransferServiceControl.GetServiceActivityStatus(GetServiceActivityStatus);
            serviceControl.GetServiceLogDelegate = new DbTransferServiceControl.GetServiceLog(GetServiceLog);
            serviceControl.SuspendServiceDelegate = new DbTransferServiceControl.SuspendService(SuspendService);
            serviceControl.ContinueServiceDelegate = new DbTransferServiceControl.ContinueService(ContinueService);
            _mimPublish2Context = serviceProvider.GetRequiredService<MIMPublish2Context>();
            _eventsAndAssingmentsContext = serviceProvider.GetRequiredService<EventsAndAssignmentsContext>();
            _serviceStartTimeList =
                configuration.GetSection("DataTransferTime").Get<TimeSpan[]>().OrderBy(ts => ts.Ticks).ToList();
            _deleteOldEmployees = configuration.GetValue<bool>("DeleteOldEmployees");
            _deleteOldOrganizations = configuration.GetValue<bool>("DeleteOldOrganizations");
            _mimConnection = new SqlConnection(connStrService.MIMPublish2ConnectionString);
            _easConnection = new SqlConnection(connStrService.EventsAndAssignmentsConnectionString);
            _logger = logger;
        }

        /// <inheritdoc cref="DbTransferServiceControl.GetServiceActivityStatus"/>
        public bool GetServiceActivityStatus()
        {
            return !_serviceOnPause;
        }

        /// <inheritdoc cref="DbTransferServiceControl.GetServiceLog"/>
        public IList<IServiceEvent> GetServiceLog()
        {
            return _serviceEventList;
        }

        /// <inheritdoc cref="DbTransferServiceControl.SuspendService"/>
        public void SuspendService(string? comment = null)
        {
            _serviceOnPause = true;
            LogServiceEvent(new ServiceSuspendedEvent(DateTime.Now, comment!));
        }

        /// <inheritdoc cref="DbTransferServiceControl.ContinueService"/>
        public void ContinueService(string? comment = null)
        {
            _serviceOnPause = false;
            LogServiceEvent(new ServiceContinuedEvent(DateTime.Now, comment!));
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Запуск сервиса...");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Остановка сервиса...");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                TimeSpan waitingTimeToNextLaunch = GetTimeoutUntilNextLaunch();

                if (_serviceOnPause)
                {
                    await Task.Delay(waitingTimeToNextLaunch, stoppingToken);
                    continue;
                }

                try
                {
                    _logger.LogInformation("Начало трансфера данных");

                    _mimConnection.Open();
                    _easConnection.Open();

                    TransferEmployeesBetweenDb(stoppingToken);
                    TransferOrganizationsBetweenDb(stoppingToken);

                    _mimConnection.Close();
                    _easConnection.Close();

                    _logger.LogInformation("Трансфер данных был завершен");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Трансфер данных не был успешно завершен: {Description}", ex.Message);
                }

                _logger.LogInformation(
                    "Следующий трансфер данных запланирован через {Delay}", waitingTimeToNextLaunch);

                await Task.Delay(waitingTimeToNextLaunch, stoppingToken);
            }
        }

        /// <summary>
        /// Получить отрезок времени до следующего запуска сервиса
        /// </summary>
        private TimeSpan GetTimeoutUntilNextLaunch()
        {
            // Ближайшее время запуска на сегодня, или полночь следующего дня
            TimeSpan nextLaunchTime = _serviceStartTimeList.Find(
                ts => DateTime.Today.Ticks + ts.Ticks > DateTime.Now.Ticks);
            TimeSpan spanUntilNextLaunch = TimeSpan.Zero;

            // Расчет оставшегося времени, если время запуска - сегодня
            if (nextLaunchTime != default)
            {
                DateTime datetimeOfNextLaunch = new(DateTime.Today.Ticks + nextLaunchTime.Ticks);
                spanUntilNextLaunch = new TimeSpan(datetimeOfNextLaunch.Ticks - DateTime.Now.Ticks);
            }
            // Расчет оставшегося времени, если время запуска - завтра
            else
            {
                nextLaunchTime = _serviceStartTimeList[0];
                DateTime datetimeOfNextLaunch = new(DateTime.Today.AddDays(1).Ticks + nextLaunchTime.Ticks);
                spanUntilNextLaunch = new TimeSpan(datetimeOfNextLaunch.Ticks - DateTime.Now.Ticks);
            }

            if (spanUntilNextLaunch.Ticks < 0)
            {
                spanUntilNextLaunch = TimeSpan.Zero;
            }

            return spanUntilNextLaunch;
        }

        /// <summary>
        /// Переносит новые данные о трудозанятых между БД-приемником и БД-источником
        /// </summary>
        /// <remarks>
        /// Берет информацию о уже имеющихся записях в БД-приемнике; Находит в БД-источнике
        /// записи, которых еще нет в БД-приемнике, либо они изменились в БД-источнике;
        /// Передает новые/измененные записи из БД-источника в БД-приемник.
        /// </remarks>
        private void TransferEmployeesBetweenDb(in CancellationToken stoppingToken)
        {
            try
            {
                if (_deleteOldEmployees)
                {
                    IEnumerable<Guid> employeesNotToRemove =
                        _easConnection.Query<Guid>(GetSqlBody("SelectEmployeesNotForRemoval"));

                    _ = _easConnection.Execute(GetSqlBody("DeleteEmployeesWithFilter"), new { employeesNotToRemove });
                }

                IEnumerable<EmployeeRecord> lastMimPublish2Data =
                    _mimConnection.Query<EmployeeRecord>(GetSqlBody("SelectEmployeesWithModificationDateFromMim"));

                List<EmployeeRecord> lastEventsAndAssignmentsData = _easConnection
                    .Query<EmployeeRecord>(GetSqlBody("SelectEmployeesWithModificationDateFromEas")).AsList();

                List<Guid> newOrRenewingEmployeePositionIds =
                    lastMimPublish2Data.Except(lastEventsAndAssignmentsData).Select(e => e.PositionId).ToList();

                int newRecords = PerformBatchEmployeeTransfer(
                    lastEventsAndAssignmentsData, newOrRenewingEmployeePositionIds, stoppingToken);

                List<Guid> actualEmployees = lastMimPublish2Data.Select(e => e.PositionId).ToList();

                SetActualEmployeesInDb(actualEmployees);

                int totalRecords = _easConnection.Query<int>(GetSqlBody("SelectEmployeesCountFromEas")).First();

                _ = TransferPhotosBetweenDb(stoppingToken);

                LogServiceEvent(new DataTransferResult(
                    DateTime.Now, true, newRecords, totalRecords, "Перенос сотрудников выполнен"));
            }
            catch (Exception exception)
            {
                LogServiceEvent(new DataTransferResult(
                    DateTime.Now, false, 0, 0, $"Ошибка при переносе сотрудников: {exception.Message}"));
            }
        }

        /// <summary>
        /// На основании контекстов БД, актуальных данных БД-приемника,и новых данных
        /// БД-источника произвести пакетный перенос данных (до 1000 штук за раз)
        /// </summary>
        private int PerformBatchEmployeeTransfer(
            List<EmployeeRecord> lastEventsAndAssignmentsData,
            List<Guid> newOrUpdatedEmployees,
            in CancellationToken stoppingToken)
        {
            int totalRecords = 0;
            const int bufferSize = 100;
            int index = 0;
            while (index < newOrUpdatedEmployees.Count)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(stoppingToken);
                }

                int countOfElementsToTake = newOrUpdatedEmployees.Count - index < bufferSize
                        ? newOrUpdatedEmployees.Count - index : bufferSize;

                List<Guid> employeeRecordsPack = newOrUpdatedEmployees.GetRange(index,
                    newOrUpdatedEmployees.Count - index < bufferSize ? newOrUpdatedEmployees.Count - index : bufferSize).ToList();
                IEnumerable<PuplicEmployeeView> updatedEmployedPack = _mimConnection.Query<PuplicEmployeeView>(
                    GetSqlBody("SelectEmployeesByIdFromMim"), new { valuesToGet = employeeRecordsPack });

                index += countOfElementsToTake;

                foreach (var employee in updatedEmployedPack)
                {
                    _ = lastEventsAndAssignmentsData.Exists(e => e.PositionId == employee.PositionId)
                        ? _easConnection.Execute(GetSqlBody("UpdateEmployeeInEas"), employee)
                        : _easConnection.Execute(GetSqlBody("InsertEmployeeInEas"), employee);
                }

                totalRecords += employeeRecordsPack.Count;

                // HACK !! Пока здесь идет как заглушка, для уменьшения потребления памяти
                //GC.Collect();
            }

            return totalRecords;
        }

        /// <summary>
        /// На основании переданного списка трудозанятых, пометить их записи в БД, как актуальные
        /// </summary>
        /// <param name="actualEmployeePositionIds">Ид</param>
        private void SetActualEmployeesInDb(List<Guid> actualEmployeePositionIds)
        {
            _ = _easConnection.Execute(GetSqlBody("SetAllEmployeesNotActual"));

            int index = 0;
            while (index < actualEmployeePositionIds.Count)
            {
                IEnumerable<Guid> elements = actualEmployeePositionIds.Skip(index).Take(1000);
                _ = _easConnection.Execute(GetSqlBody("SetActualEmployees"), new { actualEmployees = elements });
                index += 1000;
            }
        }

        /// <summary>
        /// Произвести трансфер фотографий между БД. Обновленным считается фото, у которого изменился размер
        /// </summary>
        private int TransferPhotosBetweenDb(in CancellationToken stoppingToken)
        {
            IEnumerable<(Guid employeeId, int? photoLength)> mimEmployeePhotosLength =
                _mimConnection.Query<(Guid, int?)>(GetSqlBody("SelectEmployeePhotosLengthFromMim"));

            IEnumerable<(Guid employeeId, int? photoLength)> easEmployeePhotosLength =
                _easConnection.Query<(Guid, int?)>(GetSqlBody("SelectEmployeePhotosLengthFromEas"));

            List<(Guid employeeId, int? photoLength)> employeePhotosForUpdate =
                mimEmployeePhotosLength.Except(easEmployeePhotosLength).ToList();

            int totalRecords = 0;
            const int bufferSize = 1000;
            int index = 0;
            while (index < employeePhotosForUpdate.Count)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(stoppingToken);
                }

                int countOfElementsToTake = employeePhotosForUpdate.Count - index < bufferSize
                    ? employeePhotosForUpdate.Count - index : bufferSize;

                List<Guid> employeePhotosPack = employeePhotosForUpdate.GetRange(index,
                    employeePhotosForUpdate.Count - index < bufferSize ? employeePhotosForUpdate.Count - index : bufferSize)
                    .ConvertAll(e => e.employeeId);
                IEnumerable<(Guid employeeId, byte[] photo, byte[] smallPhoto)> updatedPackOfPhotos =
                    _mimConnection.Query<(Guid employeeId, byte[], byte[])>(
                        GetSqlBody("SelectEmployeePhotosFromMim"), new { valuesToGet = employeePhotosPack });

                index += countOfElementsToTake;

                foreach (var (employeeId, photo, smallPhoto) in updatedPackOfPhotos)
                {
                    _ = _easConnection.Execute(GetSqlBody("UpdateEmployeePhotosInEas"),
                        new { photo, smallPhoto, employeeId }, commandTimeout: 60);
                }

                totalRecords += employeePhotosPack.Count;

                // HACK !! Пока здесь идет как заглушка, для уменьшения потребления памяти
                //GC.Collect();
            }

            return totalRecords;
        }

        /// <summary>
        /// Переносит новые данные об организациях между БД-приемником и БД-источником
        /// </summary>
        private void TransferOrganizationsBetweenDb(in CancellationToken stoppingToken)
        {
            try
            {
                if (_deleteOldOrganizations)
                {
                    _mimPublish2Context.PuplicOrganizationsViews
                        .RemoveRange(_mimPublish2Context.PuplicOrganizationsViews);
                }

                List<RecordModificationTime> lastMimPublish2RowsData =
                    _mimPublish2Context.PuplicOrganizationsViews
                    .Select(ovr => new RecordModificationTime(ovr.OrganizationId, ovr.LastModificationDate))
                    .AsNoTracking()
                    .ToList();

                List<RecordModificationTime> lastEventsAndAssignmentsData =
                    _eventsAndAssingmentsContext.PuplicOrganizationsViews
                    .Select(ovr => new RecordModificationTime(ovr.OrganizationId, ovr.LastModificationDate))
                    .AsNoTracking()
                    .ToList();

                List<Guid> newOrRenewOrganizations = lastMimPublish2RowsData
                    .ExceptBy(
                        lastEventsAndAssignmentsData.Select(e => (e.RecordId, e.LastModificationDate)),
                        e => (e.RecordId, e.LastModificationDate))
                    .Select(o => o.RecordId)
                    .ToList();

                List<PuplicOrganizationsView> freshRecords = _mimPublish2Context.PuplicOrganizationsViews
                    .Where(o => newOrRenewOrganizations
                    .Contains(o.OrganizationId))
                    .ToList();

                foreach (var organization in freshRecords)
                {
                    _ = lastEventsAndAssignmentsData.Exists(e => e.RecordId == organization.OrganizationId)
                        ? _eventsAndAssingmentsContext.PuplicOrganizationsViews.Update(organization)
                        : _eventsAndAssingmentsContext.PuplicOrganizationsViews.Add(organization);
                }

                int newRecords = _eventsAndAssingmentsContext.SaveChangesAsync(stoppingToken).Result;

                int totalRecords = _eventsAndAssingmentsContext.PuplicOrganizationsViews.Count();

                LogServiceEvent(new DataTransferResult(
                    DateTime.Now, true, newRecords, totalRecords, "Перенос организаций выполнен"));
            }
            catch (Exception exception)
            {
                LogServiceEvent(new DataTransferResult(
                    DateTime.Now, false, 0, 0, $"Ошибка при переносе организаций: {exception.Message}"));
            }
        }

        /// <summary>
        /// Получает тело SQL запроса из файла папки "SQL" в директории приложения
        /// </summary>
        /// <param name="file">Имя требуемого файла (SQL запроса)</param>
        private string GetSqlBody(string file)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "SQL", $"{file}.sql");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            return File.ReadAllText(path);
        }

        /// <summary>
        /// Логгирует событие сервиса во внутренний список событий и логгер приложения
        /// </summary>
        private void LogServiceEvent(IServiceEvent serviceEvent)
        {
            _serviceEventList.Add(serviceEvent);
            _logger.LogInformation(JsonSerializer.Serialize(serviceEvent));
        }
    }
}