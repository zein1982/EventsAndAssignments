using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.DTO_GottaGetOutOfHere;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Helpers;
using EventsAndAssignments.Services.Interfaces;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace EventsAndAssignments.Services.Data
{
    public class AssignmentHistoryService : IAssignmentHistoryService
    {
        private readonly IAssignmentHistoryGateway _assignmentHistoryGateway;
        private readonly IAssignmentHistoryMessageBuilderService _messageBuilderService;
        private readonly ILogger<AssignmentHistoryService> _logger;
        private readonly IMapper _mapper;

        public AssignmentHistoryService(
            IAssignmentHistoryGateway assignmentHistoryGateway,
            IAssignmentHistoryMessageBuilderService messageBuilderService,
            ILogger<AssignmentHistoryService> logger,
            IMapper mapper)
        {
            _assignmentHistoryGateway = assignmentHistoryGateway;
            _messageBuilderService = messageBuilderService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить все записи истории поручений по id поручения
        /// </summary>
        /// <param name="assignmentId">Идентификатор поручения</param>
        public async Task<ICollection<AssignmentHistoryResponseDto>> GetAll(long assignmentId)
        {
            ICollection<AssignmentHistory> historyItems = await _assignmentHistoryGateway.GetAllAsync(assignmentId);

            List<AssignmentHistoryMessage> messages = historyItems
                .Select(ToHistoryMessage)
                .Select(AddDescription)
                .ToList();

            return _mapper.Map<ICollection<AssignmentHistoryResponseDto>>(messages);
        }

        /// <summary>
        /// Записать все изменения поручения
        /// </summary>
        /// <param name="from">Поручение из БД</param>
        /// <param name="to">Поручение с фронта</param>
        /// <param name="currentEmployee">Текущий пользователь, работающий в системе</param>
        public async Task CreateFromAssignmentModificationAsync(
            Assignment from,
            Assignment to, Models.DTO.Common.Employee currentEmployee)
        {
            _ = await CreateFromAssignmentResponsibleExecutorModification(from, to, currentEmployee);
            _ = await CreateFromAssignmentResponsibleLeaderModification(from, to, currentEmployee);
            _ = await CreateFromAssignmentResponsibleInspectorModification(from, to, currentEmployee);
            _ = await CreateFromAssignmentAuthorModification(from, to, currentEmployee);
            _ = await CreateFromAssignmentStatusModification(from, to, currentEmployee);
        }

        /// <summary>
        /// Записать изменения при добавлении или удалении файлов
        /// </summary>
        /// <param name="file">файл</param>
        /// <param name="action">тип действия</param>
        /// <param name="currentEmployee">Текущий пользователь, работающий в системе</param>
        public async Task<AssignmentHistoryMessage> CreateFromAssignmentFilesModificationAsync(
            AssignmentFile file,
            FileAction action, Models.DTO.Common.Employee currentEmployee)
        {
            AssignmentHistory assignmentHistory = CreateNewAssignmentHistoryBase(file.AssignmentId, currentEmployee);

            switch (action)
            {
                case FileAction.Add:
                    assignmentHistory.ModificationType = (int)AssignmentModificationTypes.AddFile;
                    assignmentHistory.AddedFile = file.Id;
                    _logger.LogInformation(
                        "Запись в историю поручений: добавление файла к поручению с Id: {assignmentId}",
                        file.AssignmentId);
                    break;

                case FileAction.Remove:
                    assignmentHistory.ModificationType = (int)AssignmentModificationTypes.RemoveFile;
                    assignmentHistory.RemovedFile = file.Id;
                    _logger.LogInformation(
                        "Запись в историю поручений: удаление файла из поручения с Id: {assignmentId}",
                        file.AssignmentId);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }

            AssignmentHistory fromDb = await _assignmentHistoryGateway.CreateAsync(assignmentHistory);
            AssignmentHistoryMessage historyMessage = ToHistoryMessage(fromDb);
            historyMessage.AddedFile = file;
            return AddDescription(historyMessage);
        }

        /// <summary>
        /// Записать изменения статуса поручения в историю поручений
        /// </summary>
        /// <param name="from">Поручение из БД</param>
        /// <param name="to">Поручение с фронта</param>
        /// <param name="currentEmployee">Текущий пользователь, работающий в системе</param>
        public async Task<AssignmentHistoryMessage?> CreateFromAssignmentStatusModification(
            Assignment from,
            Assignment to,
            Models.DTO.Common.Employee currentEmployee)
        {
            if (from.StatusId is null || to.StatusId is null || from.StatusId == to.StatusId)
            {
                return null;
            }

            _logger.LogInformation("Запись в историю поручений: смена статуса поручения с Id: {assignmentId}", from.Id);

            AssignmentHistory assignmentHistory = CreateNewAssignmentHistoryBase(to.Id, currentEmployee);
            assignmentHistory.ModificationType = (int)AssignmentModificationTypes.ChangeStatus;
            assignmentHistory.FromStatus = (int)from.StatusId!;
            assignmentHistory.ToStatus = (int)to.StatusId!;
            AssignmentHistory fromDb = await _assignmentHistoryGateway.CreateAsync(assignmentHistory);
            AssignmentHistoryMessage historyMessage = ToHistoryMessage(fromDb);

            return AddDescription(historyMessage);
        }

        /// <summary>
        /// Записать изменения ответственного исполнителя поручения в историю поручений
        /// </summary>
        /// <param name="from">Поручение из БД</param>
        /// <param name="to">Поручение обновленное</param>
        /// <param name="currentEmployee">Текущий пользователь, работающий в системе</param>
        public async Task<AssignmentHistoryMessage?> CreateFromAssignmentResponsibleExecutorModification(
            Assignment from,
            Assignment to,
            Models.DTO.Common.Employee currentEmployee)
        {
            //Смена отвественного исполнителя
            if (from.ResponsibleExecutorId == to.ResponsibleExecutorId)
            {
                return null;
            }

            AssignmentHistory assignmentHistory = CreateNewAssignmentHistoryBase(to.Id, currentEmployee);

            //Добавили ответственного исполнителя
            if (CheckHelper.IsAdded(from.ResponsibleExecutorId, to.ResponsibleExecutorId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.AddExecutor;
                assignmentHistory.AddedResponsibleExecutor = to.ResponsibleExecutorId;
                assignmentHistory.RemovedResponsibleExecutor = default;
            }

            //Сменили ответственного исполнителя
            if (CheckHelper.IsChanged(from.ResponsibleExecutorId, to.ResponsibleExecutorId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.ChangeExecutor;
                assignmentHistory.AddedResponsibleExecutor = to.ResponsibleExecutorId;
                assignmentHistory.RemovedResponsibleExecutor = from.ResponsibleExecutorId;
            }

            //Удалили ответственного исполнителя
            if (CheckHelper.IsRemoved(from.ResponsibleExecutorId, to.ResponsibleExecutorId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.RemoveExecutor;
                assignmentHistory.AddedResponsibleExecutor = default;
                assignmentHistory.RemovedResponsibleExecutor = from.ResponsibleExecutorId;
            }

            _logger.LogInformation(
                "Запись в историю поручений: смена ответственного исполнителя в поручении с Id: {assignmentId}",
                from.Id);

            AssignmentHistory fromDb = await _assignmentHistoryGateway.CreateAsync(assignmentHistory);
            AssignmentHistoryMessage historyMessage = ToHistoryMessage(fromDb);

            return AddDescription(historyMessage);
        }

        /// <summary>
        /// Записать изменения ответственного руководителя поручения в историю поручений
        /// </summary>
        /// <param name="from">Поручение из БД</param>
        /// <param name="to">Поручение обновленное</param>
        /// <param name="currentEmployee">Текущий пользователь, работающий в системе</param>
        public async Task<AssignmentHistoryMessage?> CreateFromAssignmentResponsibleLeaderModification(
            Assignment from,
            Assignment to,
            Models.DTO.Common.Employee currentEmployee)
        {
            //Смена отвественного руководителя
            if (from.ResponsibleLeaderId == to.ResponsibleLeaderId)
            {
                return null;
            }

            AssignmentHistory assignmentHistory = CreateNewAssignmentHistoryBase(to.Id, currentEmployee);

            //Добавили ответственного руководителя
            if (CheckHelper.IsAdded(from.ResponsibleLeaderId, to.ResponsibleLeaderId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.AddLeader;
                assignmentHistory.AddedResponsibleExecutor = to.ResponsibleLeaderId;
                assignmentHistory.RemovedResponsibleExecutor = default;
            }

            //Сменили ответственного руководителя
            if (CheckHelper.IsChanged(from.ResponsibleLeaderId, to.ResponsibleLeaderId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.ChangeLeader;
                assignmentHistory.AddedResponsibleExecutor = to.ResponsibleLeaderId;
                assignmentHistory.RemovedResponsibleExecutor = from.ResponsibleLeaderId;
            }

            //Удалили ответственного руководителя
            if (CheckHelper.IsRemoved(from.ResponsibleLeaderId, to.ResponsibleLeaderId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.RemoveLeader;
                assignmentHistory.AddedResponsibleExecutor = default;
                assignmentHistory.RemovedResponsibleExecutor = from.ResponsibleLeaderId;
            }

            _logger.LogInformation(
                "Запись в историю поручений: смена ответственного руководителя в поручении с Id: {assignmentId}",
                from.Id);

            AssignmentHistory fromDb = await _assignmentHistoryGateway.CreateAsync(assignmentHistory);
            AssignmentHistoryMessage historyMessage = ToHistoryMessage(fromDb);

            return AddDescription(historyMessage);
        }

        /// <summary>
        /// Записать изменения ответственного контролера поручения в историю поручений
        /// </summary>
        /// <param name="from">Поручение из БД</param>
        /// <param name="to">Поручение обновленное</param>
        /// /// <param name="currentEmployee">Текущий пользователь, работающий в системе</param>
        public async Task<AssignmentHistoryMessage?> CreateFromAssignmentResponsibleInspectorModification(
            Assignment from,
            Assignment to,
            Models.DTO.Common.Employee currentEmployee)
        {
            //Смена отвественного контролера
            if (from.ResponsibleInspectorId == to.ResponsibleInspectorId)
            {
                return null;
            }

            AssignmentHistory assignmentHistory = CreateNewAssignmentHistoryBase(to.Id, currentEmployee);

            //Добавили ответственного контролера
            if (CheckHelper.IsAdded(from.ResponsibleInspectorId, to.ResponsibleInspectorId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.AddInspector;
                assignmentHistory.AddedResponsibleExecutor = to.ResponsibleInspectorId;
                assignmentHistory.RemovedResponsibleExecutor = default;
            }

            //Сменили ответственного контролера
            if (CheckHelper.IsChanged(from.ResponsibleInspectorId, to.ResponsibleInspectorId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.ChangeInspector;
                assignmentHistory.AddedResponsibleExecutor = to.ResponsibleInspectorId;
                assignmentHistory.RemovedResponsibleExecutor = from.ResponsibleInspectorId;
            }

            //Удалили ответственного контролера
            if (CheckHelper.IsRemoved(from.ResponsibleInspectorId, to.ResponsibleInspectorId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.RemoveInspector;
                assignmentHistory.AddedResponsibleExecutor = default;
                assignmentHistory.RemovedResponsibleExecutor = from.ResponsibleInspectorId;
            }

            _logger.LogInformation(
                "Запись в историю поручений: смена ответственного контролера в поручении с Id: {assignmentId}",
                from.Id);

            AssignmentHistory fromDb = await _assignmentHistoryGateway.CreateAsync(assignmentHistory);
            AssignmentHistoryMessage historyMessage = ToHistoryMessage(fromDb);

            return AddDescription(historyMessage);
        }

        /// <summary>
        /// Записать изменения при смене автора поручения в историю поручений
        /// </summary>
        /// <param name="from">Поручение из БД</param>
        /// <param name="to">Поручение обновленное</param>
        /// <param name="currentEmployee">Текущий пользователь, работающий в системе</param>
        public async Task<AssignmentHistoryMessage?> CreateFromAssignmentAuthorModification(
            Assignment from,
            Assignment to,
            Models.DTO.Common.Employee currentEmployee)
        {
            //Смена автора поручения
            if (from.AuthorId == to.AuthorId)
            {
                return null;
            }

            AssignmentHistory assignmentHistory = CreateNewAssignmentHistoryBase(to.Id, currentEmployee);

            //Добавили автора поручения
            if (CheckHelper.IsAdded(from.AuthorId, to.AuthorId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.AddAuthor;
                assignmentHistory.AddedResponsibleExecutor = to.AuthorId;
                assignmentHistory.RemovedResponsibleExecutor = default;
            }

            //Сменили автора поручения
            if (CheckHelper.IsChanged(from.AuthorId, to.AuthorId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.ChangeAuthor;
                assignmentHistory.AddedResponsibleExecutor = to.AuthorId;
                assignmentHistory.RemovedResponsibleExecutor = from.AuthorId;
            }

            //Удалили автора поручения
            if (CheckHelper.IsRemoved(from.AuthorId, to.AuthorId))
            {
                assignmentHistory.ModificationType = (int)AssignmentModificationTypes.RemoveAuthor;
                assignmentHistory.AddedResponsibleExecutor = default;
                assignmentHistory.RemovedResponsibleExecutor = from.AuthorId;
            }

            _logger.LogInformation(
                "Запись в историю поручений: смена автора поручения в поручении с Id: {assignmentId}",
                from.Id);

            AssignmentHistory fromDb = await _assignmentHistoryGateway.CreateAsync(assignmentHistory);
            AssignmentHistoryMessage historyMessage = ToHistoryMessage(fromDb);

            return AddDescription(historyMessage);
        }

        /// <summary>
        /// Создание объекта <seealso cref="AssignmentHistory"/> с параметрами по умолчанию
        /// </summary>
        /// <param name="assignmentId">Идентификатор поручения</param>
        private static AssignmentHistory CreateNewAssignmentHistoryBase(long? assignmentId, Models.DTO.Common.Employee currentEmployee) =>
            new()
            {
                AssignmentId = assignmentId,
                Created = DateTime.UtcNow,
                CreatedBy = currentEmployee.Id,
                ModificationType = 0,
                AddedResponsibleExecutor = null,
                RemovedResponsibleExecutor = null,
                FromStatus = null,
                ToStatus = null,
                AddedFile = null,
                RemovedFile = null
            };

        /// <summary>
        /// Преобразование объекта <see cref="AssignmentHistory"> истории </see> в объект <see cref="AssignmentHistoryMessage"/>
        /// </summary>
        /// <param name="history">Объект <see cref="AssignmentHistory"/></param>
        private AssignmentHistoryMessage ToHistoryMessage(AssignmentHistory history) =>
            new()
            {
                Id = history.Id,
                EmployeeFullName = history.CreatedByNavigation?.GetFullName(),
                Created = history.Created,
                ModificationType = history.ModificationType,
                AddedResponsibleExecutorFullName = history?.AddedResponsibleExecutorNavigation?.GetFullName(),
                RemovedResponsibleExecutorFullName = history?.RemovedResponsibleExecutorNavigation?.GetFullName(),
                FromStatus = history?.FromStatusNavigation,
                ToStatus = history?.ToStatusNavigation,
                AddedFile = history?.AddedFileNavigation,
                RemovedFile = history?.RemovedFileNavigation,
            };

        /// <summary>
        /// Добавить описание произошедшего события
        /// </summary>
        /// <param name="historyItem">Исходное сообщение для отображения <see cref="AssignmentHistoryMessage"/></param>
        /// <exception cref="InvalidOperationException"></exception>
        private AssignmentHistoryMessage AddDescription(AssignmentHistoryMessage historyItem)
        {
            string description = historyItem.ModificationType switch
            {
                (int)AssignmentModificationTypes.AddExecutor => _messageBuilderService
                    .UseAddResponsibleExecutorMessage(historyItem.AddedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.RemoveExecutor => _messageBuilderService
                    .UseRemoveResponsibleExecutorMessage(historyItem.RemovedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.ChangeExecutor => _messageBuilderService
                    .UseChangeResponsibleExecutorMessage(
                        historyItem.RemovedResponsibleExecutorFullName!,
                        historyItem.AddedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.ChangeStatus => _messageBuilderService
                    .UseChangeStatusMessage(historyItem?.FromStatus?.Name!, historyItem?.ToStatus?.Name!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.AddFile => _messageBuilderService
                    .UseAddFilesMessage(historyItem.AddedFile!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.RemoveFile => _messageBuilderService
                    .UseRemoveFilesMessage(historyItem.RemovedFile!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.AddLeader => _messageBuilderService
                    .UseAddResponsibleLeaderMessage(historyItem.AddedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.RemoveLeader => _messageBuilderService
                    .UseRemoveResponsibleLeaderMessage(historyItem.RemovedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.ChangeLeader => _messageBuilderService
                    .UseChangeResponsibleLeaderMessage(
                        historyItem.RemovedResponsibleExecutorFullName!,
                        historyItem.AddedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.AddInspector => _messageBuilderService
                    .UseAddResponsibleInspectorMessage(historyItem.AddedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.RemoveInspector => _messageBuilderService
                    .UseRemoveResponsibleInspectorMessage(historyItem.RemovedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.ChangeInspector => _messageBuilderService
                    .UseChangeResponsibleInspectorMessage(
                        historyItem.RemovedResponsibleExecutorFullName!,
                        historyItem.AddedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.AddAuthor => _messageBuilderService
                    .UseAddAssignmentAuthorMessage(historyItem.AddedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.RemoveAuthor => _messageBuilderService
                    .UseRemoveAssignmentAuthorMessage(historyItem.RemovedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                (int)AssignmentModificationTypes.ChangeAuthor => _messageBuilderService
                    .UseChangeAssignmentAuthorMessage(
                        historyItem.RemovedResponsibleExecutorFullName!,
                        historyItem.AddedResponsibleExecutorFullName!)
                    .Build()
                    .ToString(),
                _ => throw new InvalidOperationException("Не удается сопоставить тип события при формировании записи истории"),
            };

            if (!string.IsNullOrEmpty(description))
            {
                historyItem!.Description = description;
            }

            return historyItem!;
        }
    }
}