using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Data.EmailTemplate;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Helpers;
using EventsAndAssignments.Services.Interfaces;
using EventsAndAssignments.Services.Options;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventsAndAssignments.Services.Data
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationGateway _notificationGateway;
        private readonly IEmployeeGateway _employeeGateway;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<NotificationService> _logger;
        private readonly IMapper _mapper;
        private readonly IOptions<NotificationsOptions> _options;

        public NotificationService(ILogger<NotificationService> logger,
            IMapper mapper,
            INotificationGateway notificationGateway,
            IEmployeeGateway employeeGateway,
            IEmailSender emailSender,
            IOptions<NotificationsOptions> options)
        {
            _logger = logger;
            _notificationGateway = notificationGateway;
            _employeeGateway = employeeGateway;
            _emailSender = emailSender;
            _mapper = mapper;
            _options = options;
        }

        /// <summary>
        /// Отправить уведомления об изменениях в поручении
        /// </summary>
        /// <param name="from">Поручение до изменения</param>
        /// <param name="to">Поручение после изменения</param>
        public async Task<bool> SendAssignmentNotificationsAsync(Assignment from, Assignment to, Models.DTO.Common.Employee currentUser)
        {
            var hasNewNotifications = false;

            var fromStatus = (Status)from.StatusId!;
            var toStatus = (Status)to.StatusId!;

            //Отправка уведомлений всем ответственным по поручению
            if (fromStatus is Status.Assign && toStatus is Status.InWork)
            {
                //Уведомить ответственного руководителя
                if (to.ResponsibleLeader is not null)
                {
                    _ = await NewAssignmentResponsibleManager(to, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }

                //Уведомить ответственного исполнителя
                if (to.ResponsibleExecutor is not null && to.ResponsibleLeaderId != to.ResponsibleExecutorId)
                {
                    _ = await NewAssignmentResponsibleExecutor(to, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }

                //Уведомить ответственного контролёра
                if (to.ResponsibleInspector is not null)
                {
                    //согласно бизнес требованиям убрал создание периодического уведомления для контролера при переходе
                    //в статус В Работе. Создал отдельный метод создания для контролера и применил при переходе в статус Контроль
                    _ = await NewAssignmentResponsibleInspector(to, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }

                //Уведомить автора
                if (to.AuthorId != to.CreatedBy)
                {
                    _ = await NewAssignmentAuthor(to, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }
            }

            //Если ответственный руководитель был изменен в поручении и текущий статус
            //не НОВОЕ и не НАЗНАЧЕН, то сформировать уведомление для нового руководителя и автора
            if (fromStatus == toStatus
                && toStatus is not (Status.Created or Status.Assign))
            {
                //ответственный руководитель
                if (CheckHelper.IsChanged(from.ResponsibleLeaderId, to.ResponsibleLeaderId))
                {
                    _ = await NewAssignmentResponsibleManager(to, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }

                //автор
                if (CheckHelper.IsChanged(from.AuthorId, to.AuthorId))
                {
                    _ = await NewAssignmentAuthor(to, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }
            }

            //Отправка уведомлений при цикле по статусу в работе
            if (fromStatus is Status.InWork && toStatus is Status.InWork)
            {
                //отправить исполнителю если исполнитель изменился (по умолчанию руководитель сам и исполнитель)
                if (CheckHelper.IsChanged(from.ResponsibleExecutorId, to.ResponsibleExecutorId))
                {
                    _ = await NewAssignmentResponsibleExecutor(to, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }
            }

            //Отправка уведомлений при переходе в статус В работе из статуса Исполнено (вернул на доработку Руководитель)
            if (fromStatus is Status.Completed && toStatus is Status.InWork && to.ResponsibleLeaderId != to.ResponsibleExecutorId)
            {
                //Отправляем что статус изменился исполнителю
                _ = await AssignmentStatusChange(to, to.ResponsibleExecutor!.Email!, _options.Value.FrontUrl);
                hasNewNotifications = true;

                //Если срок исполнения для исполнителя истек
                if (to.ExecutorExecutionDate < DateTime.UtcNow.Date)
                {
                    //Если поручение вернули на доработку и с исполнителя сняли все уведомления о просрочке
                    //то создаем новое уведомление на основании которого на следующий день система создаст новое уведомление о просрочке
                    if (!HasExpiredNotificationsForResponsible(to, ResponsibleType.Executor)
                        && !HasPeriodicNotificationsForResponsible(to, ResponsibleType.Executor))
                    {
                        _ = await CreatePeriodicNotificationForResponsibleExecutor(to, _options.Value.FrontUrl);
                    }
                }
            }

            //Отправка уведомлений при переходе в статус В работе из статуса Готово (вернул на доработку Админ) 
            if (fromStatus is Status.Done or Status.Monitoring && toStatus is Status.InWork)
            {
                //Отправляем что статус изменился руководителю
                _ = await AssignmentStatusChange(to, to.ResponsibleLeader!.Email!, _options.Value.FrontUrl);
                hasNewNotifications = true;

                if (to.ResponsibleLeaderId != to.ResponsibleExecutorId)
                {
                    //Отправляем что статус изменился исполнителю
                    _ = await AssignmentStatusChange(to, to.ResponsibleExecutor!.Email!, _options.Value.FrontUrl);
                }

                //Если срок исполнения для исполнителя истек
                if (to.ExecutorExecutionDate < DateTime.UtcNow.Date)
                {//Если поручение вернули на доработку и с исполнителя сняли все уведомления о просрочке
                    //то создаем новое уведомление на основании которого на следующий день система создаст новое уведомление о просрочке
                    if (!HasExpiredNotificationsForResponsible(to, ResponsibleType.Executor)
                        && !HasPeriodicNotificationsForResponsible(to, ResponsibleType.Executor))
                    {
                        _ = await CreatePeriodicNotificationForResponsibleExecutor(to, _options.Value.FrontUrl);
                    }
                }
            }

            //Отправка уведомлений при переходе в статус Контроль
            if (fromStatus is Status.InWork && toStatus is Status.Monitoring)
            {
                _ = await AssignmentStatusChange(to, to.ResponsibleInspector!.Email!, _options.Value.FrontUrl); //Контролеру
                hasNewNotifications = true;

                //Если для контролера не существуют периодические уведомления,
                //то для него нужно создать новых обычных уведомлений и соответственно новых уведомлений о просрочке
                if (!HasPeriodicNotificationsForResponsible(to, ResponsibleType.Inspector)
                    && !HasExpiredNotificationsForResponsible(to, ResponsibleType.Inspector))
                {
                    //Создание периодического уведомления для контролера
                    _ = await CreatePeriodicNotificationForResponsibleInspector(to, _options.Value.FrontUrl);
                }
            }

            //Отправка уведомлений если статус Контроль, но изменился контролер 
            if (fromStatus is Status.Monitoring && toStatus is Status.Monitoring)
            {
                //отправить контролеру если контролер изменился 
                if (CheckHelper.IsChanged(from.ResponsibleInspectorId, to.ResponsibleInspectorId))
                {
                    //Создание периодического уведомления для контролера
                    _ = await NewAssignmentResponsibleInspector(to, _options.Value.FrontUrl);
                    _ = await CreatePeriodicNotificationForResponsibleInspector(to, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }

                //отправить исполнителю если исполнитель изменился 
                if (CheckHelper.IsChanged(from.ResponsibleExecutorId, to.ResponsibleExecutorId))
                {
                    _ = await NewAssignmentResponsibleExecutor(to, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }
            }

            //Если текущий пользователь назначен в поручении как проверяющий и он подтвердил что оно
            //сделано (получен статус verified), то рассылаем админу и ответственному руководителю
            if (toStatus is Status.Verified && to.IsResponsibleInspector(currentUser.Id))
            {
                //Уведомить о смене статуса поручения
                if (fromStatus != toStatus)
                {
                    _ = await AssignmentStatusChange(to, to.ResponsibleLeader!.Email!, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }
            }

            //Отправка уведомлений при переходе в статус исполнено
            if (toStatus is Status.Completed && fromStatus != toStatus)
            {
                //отправляю ответственному руководителю
                _ = await AssignmentStatusChange(to, to.ResponsibleLeader!.Email!, _options.Value.FrontUrl);
                hasNewNotifications = true;
            }

            //Отправка уведомлений при переходе в статус готово
            if (toStatus is Status.Done && fromStatus != toStatus)
            {
                _ = await AssignmentStatusChange(to, to.CreatedByNavigation!.Email!, _options.Value.FrontUrl);
                hasNewNotifications = true;

                //отправляю автору если автор не админ
                if (to.CreatedBy != to.AuthorId)
                {
                    _ = await AssignmentStatusChange(to, to.Author!.Email!, _options.Value.FrontUrl);
                }
            }

            //Удалить уведомления у ответственных, которые были исключены из поручения
            await CheckAndDeletePeriodicNotificationsFromExcludedEmployees(from, to);

            //Отправить уведомления
            if (hasNewNotifications)
            {
                return await ProcessNotifications();
            }

            return false;
        }

        public Task<bool> AddNotificationAsync(INotificationTemplate template, NotificationSettingResponseDTO settings)
        {
            try
            {
                Notification notification = template.GetNotification(settings.IsWeekly);

                bool isCreated = _notificationGateway.CreateNotification(notification);

                return Task.FromResult(isCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении уведомлений в БД и отправке");
            }

            return Task.FromResult(false);
        }

        public Task<bool> AddPeriodicNotificationAsync(IPeriodicNotificationTemplate template, NotificationSettingResponseDTO settings)
        {
            try
            {
                PeriodicNotification? notification = template.GetPeriodicNotification(settings.IsWeekly);

                if (notification is null)
                {
                    return Task.FromResult(false);
                }

                bool isCreated = _notificationGateway.CreatePeriodicNotification(notification);

                if (isCreated)
                {
                    _logger.LogInformation(
                        "Создано новое уведомление по поручению с id: {id} для пользователя c id: {employeeId}",
                        notification.AssignmentId, notification.RecipientPositionId);
                }

                return Task.FromResult(isCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении уведомлений в БД");
            }

            return Task.FromResult(false);
        }

        public async Task<bool> ProcessNotifications()
        {
            List<Notification> notificationList = await _notificationGateway.GetNotificationForSend();

            foreach (var notification in notificationList)
            {
                try
                {
                    await _emailSender.SendEmailAsync(notification.Recipient, notification.Title, notification.Body);
                    await _notificationGateway.SetNotificationProcessedAsync(notification.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError("[{Date}]Ошибка отправки уведомления {NotificationId} адрес: [{Recipient}]: {ErrorMessage}", DateTime.UtcNow, notification.Id, notification.Recipient, ex.Message);
                }
            }

            _logger.LogInformation("Успешно отправлено {count} уведомлений", notificationList.Count);
            return true;
        }

        public async Task<int> CreatePeriodicNotificationsWithExpiredExecutionDate()
        {
            List<PeriodicNotification>? notifications = await _notificationGateway
                .GetOrdinaryPeriodicNotificationsWithExpiredExecutionDateAsync();

            if (notifications is null)
            {
                _logger.LogInformation("Not have notifications with expired execution date");
                return 0;
            }

            //1. Уведомления должны приходить каждые 3 дня после появления статуса просрочено в карточке поручения. (сделано)
            //   Уведомления для всех ролей(отв.руководитель, отв.исполнитель и контролер). (сделано)
            //2.Контролеру уведомления о просрочке не должны приходить, если поручение не на нем(не в статусе «Контроль»).
            //3.Если поручение пришло на проверку(переведено в статус «Контроль») в статусе просрочено(вышел срок поручения по протоколу),
            //  то уведомления о просрочке Контролеру приходить не должны.
            //4.Если поручение пришло на проверку(переведено в статус «Контроль») без просрочки(не вышел срок поручения по протоколу), 
            //  то уведомления о просрочке Контролеру будут приходить только после просрочки установленного срока проверки.
            //5.Ответственному исполнителю не должно приходить уведомление о просрочке, если уведомление не в статусе «В работе».
            foreach (var notification in notifications)
            {
                //КОНТРОЛЕР
                //ОБЩИЙ СРОК ИСПОЛНЕНИЯ ПО ПРОТОКОЛУ - Assignment.ExecutionDate
                //Если поручение НЕ истекло в рамках общего срока исполнения по протоколу ТО Контролер получает уведомления
                if (notification.Assignment?.ExecutionDate.Date >= DateTime.UtcNow.Date)
                {
                    //Если поручение в статусе в контроль
                    if (notification.Assignment?.StatusId == (long)Status.Monitoring)
                    {
                        //То также создаем уведомление о просрочке для контролера, если его дата проверки истекла (4)
                        if (notification.Assignment?.InspectorCheckDate?.Date <= DateTime.UtcNow.Date
                            && notification.ResponsibleType is (int)ResponsibleType.Inspector)
                        {
                            _ = await CreatePeriodicNotificationWithExpiredExecutionDateForResponsibleInspector(
                                notification.Assignment,
                                _options.Value.FrontUrl);
                        }
                    }
                }

                //РУКОВОДИТЕЛЬ
                //Руководитель получает уведомление о просрочке в любом случае если его дата исполнения истекла
                if (notification.Assignment?.LeaderExecutionDate?.Date <= DateTime.UtcNow.Date
                    && notification.ResponsibleType is (int)ResponsibleType.Leader)
                {
                    //То создаем уведомление о просрочке только для Руководителя
                    _ = await CreatePeriodicNotificationWithExpiredExecutionDateForResponsibleLeader(
                        notification.Assignment!,
                        _options.Value.FrontUrl);
                }

                //ИСПОЛНИТЕЛЬ
                //Если поручение в статусе в работе
                if (notification.Assignment?.StatusId == (long)Status.InWork)
                {
                    //То также создаем уведомление о просрочке для ответственного исполнителя, если его дата исполнения истекла (4)
                    if (notification.Assignment?.ExecutorExecutionDate?.Date <= DateTime.UtcNow.Date
                        && notification.ResponsibleType is (int)ResponsibleType.Executor)
                    {
                        _ = await CreatePeriodicNotificationWithExpiredExecutionDateForResponsibleExecutor(
                            notification.Assignment,
                            _options.Value.FrontUrl);
                    }
                }
            }

            return notifications.Count;
        }

        public Task<int> UpdatePeriodicNotificationsExecutionDateAsync() =>
            _notificationGateway.UpdatePeriodicNotificationsExecutionDateAsync();

        /// <summary>
        /// Выполнить отправку периодических уведомлений
        /// </summary>
        /// <returns>Количество отправленных уведомлений</returns>
        public async Task<int> SendScheduledNotificationsAsync()
        {
            //Получаю уведомления для отправки
            List<PeriodicNotification> notificationsToSend = await GetPeriodicNotificationsToSendAsync();

            int messageCount = 0;

            if (notificationsToSend.Count == 0)
            {
                return messageCount;
            }

            messageCount = notificationsToSend.Count;
            foreach (var notificationResponseDto in notificationsToSend)
            {
                //Отправка уведомления
                if (notificationResponseDto.Recipient is null || string.IsNullOrWhiteSpace(notificationResponseDto.Recipient.Email))
                {
                    _logger.LogError("Не допустимый email получателя уведомления. Id периодического уведомления: {Id}", notificationResponseDto.Id);
                    continue;
                }

                //Формирую тело и заголовок письма согласно шаблона в рантайме тело сообщения также храниться в БД на случай ошибок
                string subject = GetSubject(notificationResponseDto);
                string body = GetBody(notificationResponseDto);
                await _emailSender.SendEmailAsync(notificationResponseDto.Recipient.Email, subject, body);

                //Рассчитываю дату следующей отправки
                DateTime newSendDate =
                    notificationResponseDto.NotificationType is (int)PeriodicNotificationType.Ordinary
                        ? NotificationsHelper.GetNextNotificationDate(notificationResponseDto.ExecutionDate)
                        : NotificationsHelper.GetNextExpiredNotificationDate();

                //Обновляю дату следующей отправки у уведомления в БД
                bool isSuccessfully = await UpdatePeriodicNotificationSendDateAsync(
                    notificationResponseDto.Id,
                    newSendDate);

                if (isSuccessfully)
                {
                    _logger.LogInformation(
                        "[{CurrentDate}] Обновлена дата отправки уведомления с Id: {PeriodicNotificationId}. "
                            + "Дата следующей отправки: {NewSendDate}", DateTime.UtcNow, notificationResponseDto.Id,
                        newSendDate);
                }
            }

            return messageCount;
        }

        public Task<List<Notification>> GetNotifications(string userEmail)
        {
            return _notificationGateway.GetNotificationAsync(userEmail);
        }

        public async Task<NotificationSettingResponseDTO> GetNotificationSettingsAsync(string userEmail)
        {
            Employee user = _employeeGateway.GetEmployeeByPredicate(e => e.Email!.Contains(userEmail))!;

            ArgumentNullException.ThrowIfNull(user, $"Пользователь не найден : {userEmail}");

            NotificationSetting? result = await _notificationGateway.GetNotificationSettingsAsync(user.PositionId);

            if (result is null)
            {
                return new NotificationSettingResponseDTO
                {
                    UserEmail = userEmail,
                    NewTitle = "Новое поручение",
                    IsNew = true,
                    WeeklyTitle = "Еженедельное напоминание",
                    IsWeekly = true,
                    StatusChangeTitle = "Изменение статуса",
                    IsStatusChange = true
                };
            }

            NotificationSettingResponseDTO settings = _mapper.Map<NotificationSettingResponseDTO>(result);

            return settings;
        }

        public async Task<bool> SetNotificationSettingsAsync(string userEmail, NotificationSettingRequestDTO request)
        {
            Employee user = _employeeGateway.GetEmployeeByPredicate(e => e.Email!.Contains(userEmail))!;

            ArgumentNullException.ThrowIfNull(user, $"Пользователь не найден : {userEmail}");

            NotificationSetting setting = _mapper.Map<NotificationSetting>(request);

            setting.UserPositionId = user.PositionId;

            bool isUpdated = await _notificationGateway.CreateOrUpdateNotificationSettingsAsync(setting);

            return isUpdated;
        }

        public Task<bool> DeleteNotificationsAsync(int daysOffset)
        {
            return _notificationGateway.DeleteNotificationsAsync(daysOffset);
        }

        public Task<bool> DeletePeriodicNotificationsAsync(int daysOffset) =>
            _notificationGateway.DeletePeriodicNotificationsAsync(daysOffset);

        public Task<int> DeleteExpiredPeriodicNotificationsAsync() =>
            _notificationGateway.DeleteExpiredPeriodicNotificationsAsync();

        public async Task<bool> RestoreNotificationsOnAssignments(Assignment assignment, Models.DTO.Common.Employee currentUser)
        {
            var hasNewNotifications = false;

            //Текущий статус
            Status currentStatus = assignment.StatusId  is null ? Status.Done : (Status)assignment.StatusId;

            //Ничего не делаем с поручениями в статусах (создано, назначено, готово)
            if (assignment.StatusId is (long)Status.Created or (long)Status.Assign or (long)Status.Done)
            {
                return hasNewNotifications;
            }

            //Если поручение в статусе В РАБОТЕ 
            if (currentStatus is Status.InWork)
            {
                //Если для контролера не существуют периодические уведомления,
                //то для него нужно создать новых обычных уведомлений и соответственно новых уведомлений о просрочке
                if (!HasPeriodicNotificationsForResponsible(assignment, ResponsibleType.Executor)
                    && !HasExpiredNotificationsForResponsible(assignment, ResponsibleType.Executor)
                    && assignment.ResponsibleLeaderId != assignment.ResponsibleExecutorId)
                {
                    //Создание периодического уведомления для исполнителя
                    _ = await CreatePeriodicNotificationForResponsibleExecutor(assignment, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }
            }

            //Если поручение в статусе КОНТРОЛЬ
            if (currentStatus is Status.Monitoring)
            {
                //Если для контролера не существуют периодические уведомления,
                //то для него нужно создать новых обычных уведомлений и соответственно новых уведомлений о просрочке
                if (!HasPeriodicNotificationsForResponsible(assignment, ResponsibleType.Inspector)
                    && !HasExpiredNotificationsForResponsible(assignment, ResponsibleType.Inspector)
                    && assignment.ResponsibleLeaderId != assignment.ResponsibleInspectorId)
                {
                    //Создание периодического уведомления для контролера
                    _ = await CreatePeriodicNotificationForResponsibleInspector(assignment, _options.Value.FrontUrl);
                    hasNewNotifications = true;
                }
            }

            //Для руководителя создаем уведомления если поручение в статусе (В РАБОТЕ, КОНТРОЛЬ, ПРОВЕРЕНО, ИСПОЛНЕНО)
            if (!HasPeriodicNotificationsForResponsible(assignment, ResponsibleType.Leader)
                && !HasExpiredNotificationsForResponsible(assignment, ResponsibleType.Leader))
            {
                //Создание периодического уведомления для руководителя
                _ = await CreatePeriodicNotificationForResponsibleLeader(assignment, _options.Value.FrontUrl);
                hasNewNotifications = true;
            }

            return hasNewNotifications;
        }

        /// <summary>
        /// Получить список периодических уведомлений для отправки
        /// </summary>
        private Task<List<PeriodicNotification>> GetPeriodicNotificationsToSendAsync() =>
            _notificationGateway.GetPeriodicNotificationsToSendAsync();

        /// <summary>
        /// Обновить дату отправки уведомления
        /// </summary>
        /// <param name="newSendDate">новая дата отправки</param>
        private async Task<bool> UpdatePeriodicNotificationSendDateAsync(long periodicNotificationId,
            DateTime newSendDate)
        {
            bool isSuccessfully = await _notificationGateway
                .UpdatePeriodicNotificationSendDateAsync(periodicNotificationId, newSendDate);

            return isSuccessfully;
        }

        /// <summary>
        /// Уведомление о смене статуса
        /// </summary>
        private async Task<bool> AssignmentStatusChange(Assignment updatedAssignment, string recipientEmail, string frontUrl)
        {
            StatusChangeTemplate template = new(updatedAssignment, recipientEmail, frontUrl);

            //Получаем настройки пользователя для уведомлений
            NotificationSettingResponseDTO settings = await GetNotificationSettingsAsync(recipientEmail);

            //Новое поручение
            bool isSuccessfully = await AddNotificationAsync(template, settings);

            return isSuccessfully;
        }

        /// <summary>
        /// Уведомление Автора по новому поручению
        /// </summary>
        private async Task<bool> NewAssignmentAuthor(Assignment updatedAssignment, string frontUrl)
        {
            AssignmentAuthorTemplate template = new(updatedAssignment, frontUrl);

            if (string.IsNullOrWhiteSpace(updatedAssignment.Author?.Email))
            {
                _logger.LogError("Not valid Email address: {mailAddress}. Assignment id: {AssignmentId}"
                    , updatedAssignment.Author?.Email
                    , updatedAssignment.Id);
                return false;
            }

            //Получаем настройки пользователя для уведомлений
            NotificationSettingResponseDTO settings = await GetNotificationSettingsAsync(updatedAssignment.ResponsibleLeader?.Email!);

            //Новое поручение
            bool isSuccessfully = await AddNotificationAsync(template, settings);

            return isSuccessfully;
        }

        /// <summary>
        /// Уведомление Ответственного руководителя по новому поручению
        /// </summary>
        private async Task<bool> NewAssignmentResponsibleManager(Assignment updatedAssignment, string frontUrl)
        {
            //Уведомление о назначении
            AssignmentResponsibleManagerTemplate template = new(updatedAssignment, frontUrl);

            if (string.IsNullOrWhiteSpace(updatedAssignment.ResponsibleLeader?.Email))
            {
                _logger.LogError("Not valid Email address: {mailAddress}. Assignment id: {AssignmentId}"
                    , updatedAssignment.ResponsibleLeader?.Email
                    , updatedAssignment.Id);
                return false;
            }

            //Получаем настройки пользователя для уведомлений
            NotificationSettingResponseDTO settings =
                await GetNotificationSettingsAsync(updatedAssignment.ResponsibleLeader?.Email!);

            //Новое поручение
            bool isSuccessfully = await AddNotificationAsync(template, settings);

            return isSuccessfully;
        }

        /// <summary>
        /// Уведомление Ответственного исполнителя по новому поручению
        /// </summary>
        private async Task<bool> NewAssignmentResponsibleExecutor(Assignment updatedAssignment, string frontUrl)
        {
            //Уведомление о назначении
            AssignmentResponsibleExecutorTemplate template = new(updatedAssignment, frontUrl);

            //Получаем настройки пользователя для уведомлений
            if (string.IsNullOrWhiteSpace(updatedAssignment.ResponsibleExecutor?.Email))
            {
                _logger.LogError("Not valid Email address: {mailAddress}. Assignment id: {AssignmentId}"
                    , updatedAssignment.ResponsibleExecutor?.Email
                    , updatedAssignment.Id);
                return false;
            }

            NotificationSettingResponseDTO settings = await GetNotificationSettingsAsync(updatedAssignment.ResponsibleExecutor?.Email!);

            //Новое поручение
            bool isSuccessfully = await AddNotificationAsync(template, settings);

            return isSuccessfully;
        }

        /// <summary>
        /// Уведомление Ответственного контролера по новому поручению (без периодического уведомления)
        /// </summary>
        private async Task<bool> NewAssignmentResponsibleInspector(Assignment updatedAssignment, string frontUrl)
        {
            //Уведомление о назначении
            AssignmentInspectorTemplate template = new(updatedAssignment, frontUrl);

            if (string.IsNullOrWhiteSpace(updatedAssignment.ResponsibleInspector?.Email))
            {
                _logger.LogError("Not valid Email address: {mailAddress}. Assignment id: {AssignmentId}"
                    , updatedAssignment.ResponsibleInspector?.Email
                    , updatedAssignment.Id);
                return false;
            }

            //Получаем настройки пользователя для уведомлений
            NotificationSettingResponseDTO settings =
                await GetNotificationSettingsAsync(updatedAssignment.ResponsibleInspector?.Email!);

            //Новое поручение
            bool isSuccessfully = await AddNotificationAsync(template, settings);

            return isSuccessfully;
        }

        /// <summary>
        /// Создание периодического уведомления для ответственного контролера
        /// </summary>
        private async Task<bool> CreatePeriodicNotificationForResponsibleInspector(Assignment updatedAssignment, string frontUrl)
        {
            //Шаблон периодического уведомления для контролера
            AssignmentInspectorPeriodicTemplate template = new(updatedAssignment, frontUrl);

            //Получаем настройки пользователя для уведомлений
            NotificationSettingResponseDTO settings = await GetNotificationSettingsAsync(updatedAssignment.ResponsibleInspector?.Email!);

            //Новое поручение
            bool isSuccessfully = await AddPeriodicNotificationAsync(template, settings);

            return isSuccessfully;
        }

        /// <summary>
        /// Создание периодического уведомления для ответственного руководителя
        /// </summary>
        private async Task<bool> CreatePeriodicNotificationForResponsibleLeader(Assignment updatedAssignment, string frontUrl)
        {
            //Шаблон периодического уведомления для контролера
            AssignmentLeaderPeriodicTemplate template = new(updatedAssignment, frontUrl);

            //Получаем настройки пользователя для уведомлений
            NotificationSettingResponseDTO settings = await GetNotificationSettingsAsync(updatedAssignment.ResponsibleLeader?.Email!);

            //Новое поручение
            bool isSuccessfully = await AddPeriodicNotificationAsync(template, settings);

            return isSuccessfully;
        }

        /// <summary>
        /// Создание периодического уведомления для ответственного контролера
        /// </summary>
        private async Task<bool> CreatePeriodicNotificationForResponsibleExecutor(Assignment updatedAssignment, string frontUrl)
        {
            //Шаблон периодического уведомления для контролера
            AssignmentExecutorPeriodicTemplate template = new(updatedAssignment, frontUrl);

            //Получаем настройки пользователя для уведомлений
            NotificationSettingResponseDTO settings = await GetNotificationSettingsAsync(updatedAssignment.ResponsibleExecutor?.Email!);

            //Новое поручение
            bool isSuccessfully = await AddPeriodicNotificationAsync(template, settings);

            return isSuccessfully;
        }

        /// <summary>
        /// Создание периодического уведомления об истечении срока для ответственного руководителя
        /// </summary>
        private async Task<bool> CreatePeriodicNotificationWithExpiredExecutionDateForResponsibleLeader(Assignment updatedAssignment, string frontUrl)
        {
            //Шаблон периодического уведомления для контролера
            ExpiredLeaderPeriodicNotificationTemplate template = new(updatedAssignment, frontUrl);

            //Получаем настройки пользователя для уведомлений
            NotificationSettingResponseDTO settings = await GetNotificationSettingsAsync(updatedAssignment.ResponsibleLeader?.Email!);

            //Новое поручение
            bool isSuccessfully = await AddPeriodicNotificationAsync(template, settings);

            if (isSuccessfully)
            {
                _logger.LogInformation(@"Success! Periodic Notification about Expired execution date for Responsible Leader
                , successfully created.");
            }

            return isSuccessfully;
        }

        /// <summary>
        /// Создание периодического уведомления об истечении срока для ответственного исполнителя
        /// </summary>
        private async Task<bool> CreatePeriodicNotificationWithExpiredExecutionDateForResponsibleExecutor(Assignment updatedAssignment, string frontUrl)
        {
            //Шаблон периодического уведомления для контролера
            ExpiredExecutorPeriodicNotificationTemplate template = new(updatedAssignment, frontUrl);

            //Получаем настройки пользователя для уведомлений
            NotificationSettingResponseDTO settings = await GetNotificationSettingsAsync(updatedAssignment.ResponsibleExecutor?.Email!);

            //Новое поручение
            bool isSuccessfully = await AddPeriodicNotificationAsync(template, settings);

            if (isSuccessfully)
            {
                _logger.LogInformation(@"Success! Periodic Notification about Expired execution date for Responsible Executor
                , successfully created.");
            }

            return isSuccessfully;
        }

        /// <summary>
        /// Создание периодического уведомления об истечении срока для ответственного контролера
        /// </summary>
        private async Task<bool> CreatePeriodicNotificationWithExpiredExecutionDateForResponsibleInspector(Assignment updatedAssignment, string frontUrl)
        {
            //Шаблон периодического уведомления для контролера
            ExpiredInspectorPeriodicNotificationTemplate template = new(updatedAssignment, frontUrl);

            //Получаем настройки пользователя для уведомлений
            NotificationSettingResponseDTO settings = await GetNotificationSettingsAsync(updatedAssignment.ResponsibleInspector?.Email!);

            //Новое поручение
            bool isSuccessfully = await AddPeriodicNotificationAsync(template, settings);

            if (isSuccessfully)
            {
                _logger.LogInformation(@"Success! Periodic Notification about Expired execution date for Responsible Inspector
                , successfully created.");
            }

            return isSuccessfully;
        }

        private bool HasExpiredNotificationsForResponsible(Assignment assignment, ResponsibleType responsibleType) =>
            responsibleType switch
            {
                ResponsibleType.Executor => assignment.PeriodicNotifications.Any(periodicNotification =>
                    periodicNotification is
                    {
                        NotificationType: (int)PeriodicNotificationType.AfterDeadline,
                        ResponsibleType: (int)ResponsibleType.Executor
                    }),
                ResponsibleType.Inspector => assignment.PeriodicNotifications.Any(periodicNotification =>
                    periodicNotification is
                    {
                        NotificationType: (int)PeriodicNotificationType.AfterDeadline,
                        ResponsibleType: (int)ResponsibleType.Inspector
                    }),
                ResponsibleType.Leader => assignment.PeriodicNotifications.Any(periodicNotification =>
                    periodicNotification is
                    {
                        NotificationType: (int)PeriodicNotificationType.AfterDeadline,
                        ResponsibleType: (int)ResponsibleType.Leader
                    }),
                _ => false
            };

        private bool HasPeriodicNotificationsForResponsible(Assignment assignment, ResponsibleType responsibleType) =>
            responsibleType switch
            {
                ResponsibleType.Executor => assignment.PeriodicNotifications.Any(periodicNotification =>
                    periodicNotification is
                    {
                        NotificationType: (int)PeriodicNotificationType.Ordinary,
                        ResponsibleType: (int)ResponsibleType.Executor
                    }),
                ResponsibleType.Inspector => assignment.PeriodicNotifications.Any(periodicNotification =>
                    periodicNotification is
                    {
                        NotificationType: (int)PeriodicNotificationType.Ordinary,
                        ResponsibleType: (int)ResponsibleType.Inspector
                    }),
                ResponsibleType.Leader => assignment.PeriodicNotifications.Any(periodicNotification =>
                    periodicNotification is
                    {
                        NotificationType: (int)PeriodicNotificationType.Ordinary,
                        ResponsibleType: (int)ResponsibleType.Leader
                    }),
                _ => false
            };

        private string GetBody(PeriodicNotification notification)
        {
            if (notification.Assignment is not null)
            {
                return (PeriodicNotificationType)notification.NotificationType switch
                {
                    PeriodicNotificationType.Ordinary => (ResponsibleType)notification.ResponsibleType switch
                    {
                        ResponsibleType.None => notification.Message,
                        ResponsibleType.Leader => TemplateUtils.GetHtmlFormattedNotificationBody(
                            notification.Assignment, "Ответственного руководителя", notification.Assignment.LeaderExecutionDate, _options.Value.FrontUrl, "Напоминаем, что"),
                        ResponsibleType.Executor => TemplateUtils.GetHtmlFormattedNotificationBody(
                            notification.Assignment, "Ответственного исполнителя", notification.Assignment.ExecutorExecutionDate, _options.Value.FrontUrl, "Напоминаем, что"),
                        ResponsibleType.Inspector => TemplateUtils.GetHtmlFormattedNotificationBody(
                            notification.Assignment, "Контролера", notification.Assignment.InspectorCheckDate, _options.Value.FrontUrl, "Напоминаем, что"),
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    PeriodicNotificationType.AfterDeadline => (ResponsibleType)notification.ResponsibleType switch
                    {
                        ResponsibleType.None => notification.Message,
                        ResponsibleType.Leader => TemplateUtils.GetHtmlFormattedExpiredNotificationBody(
                            notification.Assignment, "Ответственного руководителя", notification.Assignment.LeaderExecutionDate, _options.Value.FrontUrl),
                        ResponsibleType.Executor => TemplateUtils.GetHtmlFormattedExpiredNotificationBody(
                            notification.Assignment, "Ответственного исполнителя", notification.Assignment.ExecutorExecutionDate, _options.Value.FrontUrl),
                        ResponsibleType.Inspector => TemplateUtils.GetHtmlFormattedExpiredNotificationBody(
                            notification.Assignment, "Контролера", notification.Assignment.InspectorCheckDate, _options.Value.FrontUrl),
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            _logger.LogError(@"Для формирования тела уведомления должно быть загружена связная сущность поручения. 
                    Беру тело уведомления из БД.PeriodicNotification.Assignment is null");
            //Если не заинклудилось поручение то беру сообщение из БД
            return notification.Message;
        }

        private string GetSubject(PeriodicNotification notification)
        {
            if (notification.Assignment is not null)
            {
                return (PeriodicNotificationType)notification.NotificationType switch
                {
                    PeriodicNotificationType.Ordinary => TemplateUtils.GetNotificationSubject(notification.Assignment,
                        "Напоминаем."),
                    PeriodicNotificationType.AfterDeadline => TemplateUtils.GetNotificationSubject(
                        notification.Assignment,
                        "Просрочено."),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            _logger.LogError(@"Для формирования заголовка уведомления должно быть загружена связная сущность поручения. 
                    Беру заголовок уведомления из БД. PeriodicNotification.Assignment is null");
            return notification.Message;
        }

        /// <summary>
        /// Удалить периодические уведомления у сотрудников, которые были исключены из поручения
        /// </summary>
        /// <param name="from">Поручение до изменения</param>
        /// <param name="to">Поручение после изменения</param>
        private async Task CheckAndDeletePeriodicNotificationsFromExcludedEmployees(Assignment from, Assignment to)
        {
            //Изменили ответственного исполнителя
            if (CheckHelper.IsChanged(from.ResponsibleExecutorId, to.ResponsibleExecutorId))
            {
                _ = await _notificationGateway
                    .DeletePeriodicNotificationsByParamsAsync(from.ResponsibleExecutorId!.Value, from.Id,
                        ResponsibleType.Executor);
            }

            //Изменили ответственного контролера
            if (CheckHelper.IsChanged(from.ResponsibleInspectorId, to.ResponsibleInspectorId))
            {
                _ = await _notificationGateway
                    .DeletePeriodicNotificationsByParamsAsync(from.ResponsibleInspectorId!.Value, from.Id,
                        ResponsibleType.Inspector);
            }

            //Изменили ответственного руководителя
            if (CheckHelper.IsChanged(from.ResponsibleLeaderId, to.ResponsibleLeaderId))
            {
                _ = await _notificationGateway
                    .DeletePeriodicNotificationsByParamsAsync(from.ResponsibleLeaderId!.Value, from.Id,
                        ResponsibleType.Leader);
            }
        }
    }
}