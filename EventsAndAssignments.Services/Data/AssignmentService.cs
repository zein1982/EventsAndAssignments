using System.ComponentModel.DataAnnotations;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Exceptions;
using EventsAndAssignments.Services.Interfaces;
using EventsAndAssignments.Services.Sorts;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace EventsAndAssignments.Services.Data
{
    public class AssignmentsService : IAssignmentsService
    {
        private readonly IAssignmentsGateway _assignmentGateway;
        private readonly IAssignmentHistoryService _assignmentHistoryService;
        private readonly IEmployeeService _employeeService;
        private readonly ICommentService _commentService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AssignmentsService> _logger;

        public AssignmentsService(
            IAssignmentsGateway repository,
            IAssignmentHistoryService assignmentHistoryService,
            IEmployeeService employeeService,
            ICommentService commentService,
            IFileService fileService,
            IMapper mapper,
            INotificationService notificationService,
            ILogger<AssignmentsService> logger)
        {
            _assignmentGateway = repository;
            _assignmentHistoryService = assignmentHistoryService;
            _employeeService = employeeService;
            _mapper = mapper;
            _commentService = commentService;
            _fileService = fileService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<AssignmentResponse?> GetAssignmentById(long id, string? currentUserMail = "")
        {
            Assignment? result = await _assignmentGateway.GetAssignmentByIdAsync(id);

            if (result is null)
            {
                return null;
            }

            AssignmentResponse response = _mapper.Map<AssignmentResponse>(result);

            //Добавляю ответственных в ответ (мапстер не умеет)
            MapAllResponsibleEmployee(result!, response!);

            response.UserCanAddComment = false;
            if (string.IsNullOrWhiteSpace(currentUserMail))
            {
                return response;
            }

            Models.DTO.Common.Employee? currentEmployee =
                _employeeService.GetEmployeeByEmail(currentUserMail)?? throw new EntityNotFoundException();

            bool hasPermissionToAddComment = await HasPermissionToAddComment(result, currentEmployee);
            response.UserCanAddComment = hasPermissionToAddComment;

            return response;
        }

        public async Task<AssignmentResponse?> GetAssignmentByGroupIdAndVersionAsync(long groupId, int version, int subversion)
        {
            Assignment? result = await _assignmentGateway
                .GetAssignmentByGroupIdAndVersionAsync(groupId, version, subversion);

            if (result is null)
            {
                return null;
            }

            AssignmentResponse? response = _mapper.Map<AssignmentResponse?>(result!);

            //Добавляю ответственных в ответ (мапстер не умеет)
            MapAllResponsibleEmployee(result!, response!);

            return response;
        }

        public async Task<ICollection<AssignmentVersionResponse>> GetAllAssignmentVersions([Required] long assignmentId)
        {
            ICollection<Assignment> assignments = await _assignmentGateway.GetAssignmentsByGroupId(assignmentId);
            ICollection<AssignmentVersionResponse> response = _mapper.Map<ICollection<AssignmentVersionResponse>>(assignments);

            return response.Count > 0 ? response : throw new EntityNotFoundException(assignmentId);
        }

        public Task<int> GetAssignmentCountAsync(Func<Assignment, bool>? predicate = null)
        {
            return _assignmentGateway.GetAssignmentCountAsync(predicate);
        }

        public async Task<AssignmentStatusResponse> GetAssignmentStatusByStatusCodeAsync(int statusCode)
        {
            AssignmentStatus status = await _assignmentGateway.GetAssignmentStatusByStatusCode(statusCode);
            AssignmentStatusResponse response = _mapper.Map<AssignmentStatusResponse>(status);

            return response;
        }

        public async Task<ICollection<AssignmentStatusResponse>> GetAllAssignmentStatusesAsync(bool hasResponsibleInspector)
        {
            ICollection<AssignmentStatus> statuses = await _assignmentGateway.GetAllAssignmentStatusesAsync(hasResponsibleInspector);
            ICollection<AssignmentStatusResponse> response =
                _mapper.Map<ICollection<AssignmentStatusResponse>>(statuses);

            return response;
        }

        public async Task<AssignmentResponseShort> CreateAssignmentAsync(long protocolId, string currentEmployeeEmail)
        {
            Models.DTO.Common.Employee? currentEmployee =
                _employeeService.GetEmployeeByEmail(currentEmployeeEmail) ?? throw new EntityNotFoundException();

            Assignment newAssignment = CreateAssignmentBase(protocolId, currentEmployee.Id);
            Assignment created = await _assignmentGateway.CreateAssignmentAsync(newAssignment);

            //Задаю идентификатор группы для вновь созданного поручения
            created.GroupId = created.Id;
            Assignment updated = await _assignmentGateway.UpdateAssignmentAsync(created);

            //Сортирую и переименовываю все поручения в рамках протокола
            await _assignmentGateway.RenameAndOrderAllAssignmentsInProtocol(updated.ProtocolId);

            return _mapper.Map<AssignmentResponseShort>(updated);
        }

        public async Task<AssignmentResponseShort> UpdateAssignmentAsync(
            AssignmentShortRequestDto assignmentRequest,
            string currentUserEmail,
            bool? needToReturnForRevision)
        {
            Assignment mapped = _mapper.Map<Assignment>(assignmentRequest);

            //Выявил баг уже при эксплуатации нужно обновить поля короткой формы
            //поручений иначе они затрут изменения в полной форме (только те, которых нет)
            Assignment? unUpdatedAssignment = await _assignmentGateway
                .GetAssignmentByIdAsync(mapped.Id);
            if (unUpdatedAssignment is not null)
            {
                mapped.AuthorId = unUpdatedAssignment.AuthorId;
                mapped.ResponsibleExecutorId = unUpdatedAssignment.ResponsibleExecutorId;
                mapped.ExecutorExecutionDate = unUpdatedAssignment.ExecutorExecutionDate;
                mapped.ResponsibleInspectorId = unUpdatedAssignment.ResponsibleInspectorId;
                mapped.InspectorCheckDate = unUpdatedAssignment.InspectorCheckDate;
                mapped.CompletionDate = unUpdatedAssignment.CompletionDate;
                mapped.LeaderExecutionDate = unUpdatedAssignment.LeaderExecutionDate;
                mapped.OrganizationId = unUpdatedAssignment.OrganizationId;
                mapped.StatusId = unUpdatedAssignment.StatusId;
                mapped.GroupId = unUpdatedAssignment.GroupId;
            }

            List<Assignment> assignments = new() { mapped };
            Assignment updated = await UpdateOrCreateAssignmentsAsync(assignments, currentUserEmail, needToReturnForRevision);
            AssignmentResponseShort response = _mapper.Map<AssignmentResponseShort>(updated);

            //Если комментарий не был добавлен, то возвращаю созданное поручение
            //if (string.IsNullOrWhiteSpace(assignmentRequest.Comment))
            //{
            //    return response;
            //}

            //Если комментарий уже есть и он совпадает то не создаем вновь
            //по доп логике если приходит пустой коммент, то новый коммент не создается, а происходит обновление
            //старого коммента
            CommentResponseDto? last = await _commentService.GetLastAsync(assignmentRequest.Id);

            if (last is not null && last.Content == assignmentRequest.Comment)
            {
                return response;
            }

            if (last is not null && assignmentRequest.Comment?.Length == 0)
            {
                await _commentService.UpdateCommentAsync(last.Id, string.Empty);
            }

            //Сохраняю комментарий администратора 
            CommentResponseDto comment = await _commentService.CreateAsync(assignmentRequest.Comment, response.Id, currentUserEmail, response.Status);
            response.Comment = comment.Content;

            return response;
        }

        public async Task<AssignmentResponse> UpdateAssignmentAsync(AssignmentRequestDto assignmentRequestDto,
            string currentUserEmail, bool? needToReturnForRevision)
        {
            IList<Assignment> assignmentList = ReproduceAndUpdateAssignmentByResponsibleLeaders(assignmentRequestDto);
            IList<Assignment> updatedList = new List<Assignment>();

            //Возвращаю в любом случае родительское поручение
            Assignment parent =
                await UpdateOrCreateAssignmentsAsync(assignmentList.ToList(), currentUserEmail, needToReturnForRevision!);
            AssignmentResponse response = _mapper.Map<AssignmentResponse>(parent);

            //Добавляю ответственных в ответ (мапстер не умеет)
            MapAllResponsibleEmployee(parent, response);

            return response;
        }

        public async Task RemoveAssignmentsAsync(IReadOnlyCollection<long> ids)
        {
            await _assignmentGateway.RemoveAssignmentsAsync(ids);
        }

        public async Task<(List<AssignmentResponseShort> items, int count)> GetFilteredAssignments(RequestParams filter, string userMail)
        {
            Models.DTO.Common.Employee? employee = _employeeService.GetEmployeeByEmail(userMail);
            filter.RoleId = employee?.RoleId;
            filter.PositionId = employee?.Id;

            //Получаю все должности текущего пользователя
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles = await _employeeService
                .GetAllEmployeePositionsByPositionId(employee.Id);

            (List<Assignment> items, int count) filtered = _assignmentGateway.GetFilteredAssignments(filter, currentEmployeeAllPositionsWithRoles);

            foreach (var assignment in filtered.items)
            {
                if (
                    assignment.Comments?.Count > 0
                        && assignment.StatusId >= 3
                        && (assignment.Comments.First().StatusCreated < 3)
                        && assignment.Comments.First().CreatedBy == assignment.CreatedBy)
                {
                    assignment.Comments.First().Content = string.Empty;
                }
            }

            List<AssignmentResponseShort> response = _mapper.Map<List<AssignmentResponseShort>>(filtered.items);

            return (response, filtered.count);
        }

        public async Task<List<long>> GetFilteredAssignmentsIds(RequestParams filter, string userMail)
        {
            Models.DTO.Common.Employee? employee = _employeeService.GetEmployeeByEmail(userMail);
            filter.RoleId = employee?.RoleId;
            filter.PositionId = employee?.Id;

            //Получаю все должности текущего пользователя
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles = await _employeeService
                .GetAllEmployeePositionsByPositionId(employee!.Id);

            List<long> filtered = _assignmentGateway.GetFilteredAssignmentsIds(filter, currentEmployeeAllPositionsWithRoles);

            List<long> response = _mapper.Map<List<long>>(filtered);

            return response;
        }

        public async Task<ICollection<AssignmentResponseShort>> CopyAssignmentsAsync(
            ICollection<long> assignmentIds, long protocolId, string currentEmployeeEmail)
        {
            Models.DTO.Common.Employee currentEmployee =
                _employeeService.GetEmployeeByEmail(currentEmployeeEmail) ?? throw new EntityNotFoundException();

            List<Assignment> assignments = new();
            foreach (var id in assignmentIds)
            {
                Assignment assignment = await _assignmentGateway.GetAssignmentByIdAsync(id)
                    ?? throw new EntityNotFoundException(id);

                Assignment newAssignment = new()
                {
                    CreatedBy = currentEmployee.Id,
                    Name = BuildAssignmentName(protocolId),
                    Description = assignment.Description,
                    Version = assignment.Version+1,
                    Subversion = 0,
                    OrganizationId = assignment.OrganizationId,
                    GroupId = assignment.GroupId is 0
                        ? assignment.Id
                        : assignment.GroupId,
                    ResponsibleLeaderId = assignment.ResponsibleLeaderId,
                    ExecutionDate = DateTime.UtcNow,
                    ProtocolId = protocolId,
                    StatusId = 2,
                };
                _ = await _assignmentGateway.CreateAssignmentAsync(newAssignment);

                assignments.Add(newAssignment);
            }

            //переименовываю все поручения в рамках протокола
            await _assignmentGateway.RenameAndOrderAllAssignmentsInProtocol(protocolId);

            ICollection<AssignmentResponseShort> response = _mapper
                .Map<ICollection<AssignmentResponseShort>>(assignments);

            return response;
        }

        public async Task<string> RestoreNotificationsOnAssignments(ICollection<long> assignmentsIds, string currentUserEmail)
        {
            Models.DTO.Common.Employee currentEmployee =
                _employeeService.GetEmployeeByEmail(currentUserEmail) ?? throw new EntityNotFoundException();

            if (currentEmployee.RoleId != 1)
            {
                return "Только администратор системы может восстанавливать уведомления для поручений";
            }

            int assignmentCount = 0;
            foreach (var assignmentsId in assignmentsIds)
            {
                Assignment? assignment = await _assignmentGateway.GetAssignmentByIdAsync(assignmentsId);
                if (assignment is null)
                {
                    continue;
                }

                bool isRestored = await _notificationService.RestoreNotificationsOnAssignments(assignment, currentEmployee);
                if (isRestored)
                {
                    assignmentCount++;
                }
            }

            return await Task.FromResult($"Восстановлены уведомления для {assignmentCount} поручений");
        }

        private static void UpdateAssignment(Assignment source, Assignment target, Models.DTO.Common.Employee currentEmployee)
        {
            target.UpdatedBy = currentEmployee.Id;
            target.ExecutionDate = source.ExecutionDate;
            target.LeaderExecutionDate = source.LeaderExecutionDate;
            target.ExecutorExecutionDate = source.ExecutorExecutionDate;
            target.InspectorCheckDate = source.InspectorCheckDate;
            target.CompletionDate = source.CompletionDate;
            target.Description = source.Description;
            target.ProtocolId = source.ProtocolId;
            target.StatusId = source.StatusId;
            target.Status = source.Status;
            target.OrganizationId = source.OrganizationId;
            target.Organization = default;
            target.AuthorId = source.AuthorId;
            target.Author = default;
            target.ResponsibleLeaderId = source.ResponsibleLeaderId;
            target.ResponsibleLeader = default;
            target.ResponsibleExecutorId = source.ResponsibleExecutorId;
            target.ResponsibleExecutor = default;
            target.ResponsibleInspectorId = source.ResponsibleInspectorId;
            target.ResponsibleInspector = default;
            target.CreatedByNavigation = default;
            target.Files = null;
        }

        private void MapAllResponsibleEmployee(Assignment from, AssignmentResponse to)
        {
            if (from.ResponsibleLeader is not null)
            {
                to.ResponsibleLeaders = new List<ResponsibleResponse>
                {
                    MapToResponsible(from.ResponsibleLeader, from.LeaderExecutionDate)
                };
            }

            if (from.ResponsibleExecutor is not null)
            {
                to.ResponsibleExecutors = new List<ResponsibleResponse>
                {
                    MapToResponsible(from.ResponsibleExecutor, from.ExecutorExecutionDate)
                };
            }

            if (from.ResponsibleInspector is not null)
            {
                to.ResponsibleInspectors = new List<ResponsibleResponse>
                {
                    MapToResponsible(from.ResponsibleInspector, from.InspectorCheckDate)
                };
            }
        }

        private ResponsibleResponse MapToResponsible(Employee employee, DateTime? executionDate) =>
            new()
            {
                Employee = _mapper.Map<Models.DTO.Common.Employee?>(employee),
                ExecutionDate = executionDate
            };

        private async Task<Assignment> UpdateOrCreateAssignmentsAsync(List<Assignment> updatedAssignments,
                                            string currentUserEmail, bool? needToReturnForRevision)
        {
            //Если список пуст то выбрасываю исключение
            ArgumentNullException.ThrowIfNull(updatedAssignments);

            Models.DTO.Common.Employee currentEmployee =
                _employeeService.GetEmployeeByEmail(currentUserEmail) ?? throw new EntityNotFoundException();

            Assignment parentAssignment = updatedAssignments[0];

            Assignment unUpdatedAssignment = await _assignmentGateway.GetAssignmentByIdAsync(parentAssignment.Id)
                ?? throw new EntityNotFoundException(parentAssignment.Id);

            if (unUpdatedAssignment.ProtocolId != parentAssignment.ProtocolId)
            {
                throw new InvalidOperationException("Нельзя перемещать поручения между протоколами в рамках обновления. "
                    + "Для этого используй копирование поручений!");
            }

            //Получаем новый статус поручения
            AssignmentStatus nextStatus = await GetNextAssignmentStatus(
                unUpdatedAssignment,
                parentAssignment,
                currentEmployee,
                needToReturnForRevision);

            //Получим копию необновленного поручения
            Assignment unUpdatedCopy = unUpdatedAssignment.GetCopy();
            List<Assignment> updatedList = new();
            foreach (var updatedAssignment in updatedAssignments)
            {
                //Обновляю статус
                updatedAssignment.Status = nextStatus;
                updatedAssignment.StatusId = nextStatus.Id;

                updatedAssignment.CompletionDate = nextStatus.Id == 7 ? DateTime.UtcNow : null;

                //Если это размноженная копия то создаем ее
                if (updatedAssignment is { Subversion: > 0, Id: 0 })
                {
                    //Генерирую имя для поручения в рамках протокола
                    updatedAssignment.Name = BuildAssignmentName(updatedAssignment.ProtocolId);

                    //Создаю новое поручение
                    Assignment reproduced = await _assignmentGateway.CreateAssignmentAsync(updatedAssignment);

                    //Копируем комментарии с родительского поручения
                    await CopyCommentsFromParentAssignmentAsync(unUpdatedAssignment, reproduced.Id, currentUserEmail);

                    //Копируем файлы с родительского поручения
                    await CopyFilesFromParentAssignmentAsync(unUpdatedAssignment.Id, reproduced.Id, currentUserEmail);

                    updatedList.Add(reproduced);

                    //переименовываю все поручения в рамках протокола
                    await _assignmentGateway.RenameAndOrderAllAssignmentsInProtocol(updatedAssignment.ProtocolId);
                }
                else//Иначе обновляю сущность и записываю в БД
                {
                    UpdateAssignment(updatedAssignment, unUpdatedAssignment, currentEmployee);
                    updatedList.Add(await _assignmentGateway.UpdateAssignmentAsync(unUpdatedAssignment));
                }

                //Получаем обновленное поручение
                Assignment updatedFromDb = await _assignmentGateway.GetAssignmentByIdAsync(updatedList[^1].Id)
                    ?? throw new EntityNotFoundException(updatedList[^1].Id);

                //Записываем и историю изменения 
                await _assignmentHistoryService.CreateFromAssignmentModificationAsync(unUpdatedCopy, updatedFromDb, currentEmployee);

                //Отправить уведомления
                _ = await _notificationService.SendAssignmentNotificationsAsync(unUpdatedCopy, updatedFromDb, currentEmployee);
            }

            return updatedList[0];
        }

        private string BuildAssignmentName(long protocolId)
        {
            long number = _assignmentGateway.GetAssignmentsCountInProtocol(protocolId) + 1;

            return number.ToString();
        }

        private Assignment CreateAssignmentBase(long protocolId, Guid currentEmployee)
        {
            Assignment assignment = new()
            {
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                Removed = default,
                CreatedBy = currentEmployee,
                UpdatedBy = currentEmployee,
                IsArchived = false,
                ExecutionDate = DateTime.UtcNow,
                LeaderExecutionDate = default,
                ExecutorExecutionDate = default,
                InspectorCheckDate = default,
                CompletionDate = default,
                GroupId = 0,
                Description = string.Empty,
                ProtocolId = protocolId,
                Protocol = default,
                OrganizationId = default,
                Organization = default,
                AuthorId = null,
                StatusId = (long)Status.Created,
                Status = default,
                ResponsibleLeaderId = default,
                ResponsibleExecutorId = default,
                ResponsibleInspectorId = default,
                Author = default,
                ResponsibleLeader = default,
                ResponsibleExecutor = default,
                ResponsibleInspector = default,
                Subversion = 0,
                Version = 1,
                History = default,
                Comments = default,
                Files = default,
                Name = BuildAssignmentName(protocolId)
            };
            return assignment;
        }

        /// <summary>
        /// Размножение и обновление поручения
        /// </summary>
        /// <param name="assignmentDto">Объект содержащий данные с формы поручения</param>
        private IList<Assignment> ReproduceAndUpdateAssignmentByResponsibleLeaders(AssignmentRequestDto assignmentDto)
        {
            List<Assignment> assignments = new();

            for (int i = 0; i < assignmentDto.ResponsibleLeaders.Count; i++)
            {
                //Создаем поручение согласно данным с формы
                Assignment newAssignment = _mapper.Map<Assignment>(assignmentDto);

                //Можем размножать только исходные поручения (не размноженные)
                if (newAssignment.Subversion is 0)
                {
                    //Заполняем поле идентификатора группы поручений (поручения одной основной версии)
                    newAssignment.GroupId = newAssignment.Id;
                    //Увеличиваю сабверсию поручения (каждая сабверсия на отдельного отвественного руководителя)
                    newAssignment.Subversion = i;
                }

                //Если ответственных больше 1 то у последующих обнуляем id и автора обновления для корректного создания в БД
                if (i > 0)
                {
                    newAssignment.Id = 0;
                    newAssignment.UpdatedBy = null;
                }

                //Сейчас реализована логика что при множестве отв. руководителей может быть различное
                //количество ответственных исполнителей и контролеров.
                newAssignment.ResponsibleLeaderId = assignmentDto.ResponsibleLeaders[i]?.EmployeePositionId;
                newAssignment.ResponsibleInspectorId = assignmentDto.ResponsibleInspectors.Count > i
                    ? assignmentDto.ResponsibleInspectors[i]?.EmployeePositionId
                    : default;
                newAssignment.ResponsibleExecutorId = assignmentDto.ResponsibleExecutors.Count > i
                    ? assignmentDto.ResponsibleExecutors[i]?.EmployeePositionId
                    : default;

                newAssignment.LeaderExecutionDate = assignmentDto.ResponsibleLeaders[i]?.ExecutionDate;
                newAssignment.InspectorCheckDate = assignmentDto.ResponsibleInspectors.Count > i
                    ? assignmentDto.ResponsibleInspectors[i]?.ExecutionDate
                    : default;
                newAssignment.ExecutorExecutionDate = assignmentDto.ResponsibleExecutors.Count > i
                    ? assignmentDto.ResponsibleExecutors[i]?.ExecutionDate
                    : default;

                assignments.Add(newAssignment);
            }

            return assignments;
        }

        private async Task<AssignmentStatus> GetNextAssignmentStatus(
            Assignment unUpdated,
            Assignment updated,
            Models.DTO.Common.Employee currentEmployee,
            bool? needToReturnForRevision)
        {
            Status nextStatus = 0;
            Status currentAssignmentStatus = unUpdated.Status is null
                ? (Status)unUpdated.StatusId!
                : (Status)unUpdated.Status.StatusCode;

            //Добавили неявный признак уже в стадии эксплуатации по просьбе заказчика,
            //поручение должно оставаться в текущем статусе если передан признак needToReturnForRevision со значением null
            if (needToReturnForRevision is null && currentAssignmentStatus is not Status.Created)
            {
                return await _assignmentGateway.GetAssignmentStatusByStatusCode((int)unUpdated.StatusId!);
            }

            //Вернуть поручение на доработку
            if (needToReturnForRevision is not null
                && needToReturnForRevision.Value
                && currentAssignmentStatus is not Status.Assign
                && currentAssignmentStatus is not Status.InWork
                && currentAssignmentStatus is not Status.Created
                )
            {
                return await _assignmentGateway.GetAssignmentStatusByStatusCode((int)MoveStatusBack());
            }

            //Получаю все должности текущего пользователя
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles = await _employeeService
                .GetAllEmployeePositionsByPositionId(currentEmployee.Id);

            //Получаю возможные роли текущего пользователя в карточке поручения 
            bool isSuperAdmin = currentEmployee.RoleId is 1
                || currentEmployeeAllPositionsWithRoles.Values.Any(role => role == 1);
            bool isAdmin = updated.IsAdmin(currentEmployee.Id) || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == updated.CreatedBy);
            bool isAuthor = updated.IsAuthor(currentEmployee.Id) || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == updated.AuthorId);
            bool isResponsibleLeader = updated.IsResponsibleLeader(currentEmployee.Id)
                || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == updated.ResponsibleLeaderId);
            bool isResponsibleExecutor = updated.IsResponsibleExecutor(currentEmployee.Id)
                || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == updated.ResponsibleExecutorId);
            bool isResponsibleInspector = updated.IsResponsibleInspector(currentEmployee.Id)
                || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == updated.ResponsibleInspectorId);
            bool isSecondAdmin = _assignmentGateway.GetFolderAllowedEmployees(updated.Id)
                .Any(id => id == currentEmployee.Id
                    || currentEmployeeAllPositionsWithRoles.Keys.Any(guid => guid == id));

            //Продвигать поручения по статусам может только сотрудник являющийся ответственным или админом (может двигать любой статус)
            //разные типы ответственных могуд двигать различные статусы, иначе поручение остается в том же статусе
            switch (currentAssignmentStatus)
            {
                case Status.Created://двигает дальше только админ
                    nextStatus = updated.ResponsibleLeaderId switch
                    {
                        null => Status.Created,
                        _ => isAdmin
                            || isSecondAdmin
                            || isSuperAdmin ? Status.Assign : Status.Created
                    };
                    break;

                case Status.Assign://двигает дальше только администратор или супер администратор
                    nextStatus = updated.AuthorId switch
                    {
                        null => Status.Assign,
                        _ => isAdmin
                            || isSecondAdmin
                            || isSuperAdmin
                            ? Status.InWork
                            : Status.Assign
                    };
                    break;

                case Status.InWork:
                    if (updated.ResponsibleInspectorId is null)
                    {
                        if (isResponsibleLeader
                            || isAdmin
                            || isSecondAdmin
                            || isSuperAdmin)
                        {
                            nextStatus = Status.Done;
                        }
                        else if (isResponsibleExecutor)
                        {
                            nextStatus = Status.Completed;
                        }
                    }
                    else
                    {
                        nextStatus = Status.Monitoring;
                    }

                    break;

                case Status.Monitoring://двигает дальше только администратор и контролер и супер администратор
                    nextStatus = isResponsibleInspector
                        || isAdmin
                        || isSecondAdmin
                        || isSuperAdmin
                        ? Status.Done
                        : Status.Monitoring;

                    break;

                case Status.Verified://Устарело, исключено из маршрута
                    nextStatus = isResponsibleLeader
                        || isAdmin
                        || isSecondAdmin
                        || isSuperAdmin
                        ? Status.Completed
                        : Status.Verified;
                    break;

                case Status.Completed://двигает дальше только администратор, автор поручения или супер администратор
                    nextStatus = isAuthor
                        || isAdmin
                        || isSecondAdmin
                        || isResponsibleLeader
                        || isSuperAdmin
                        ? Status.Done
                        : Status.Completed;
                    break;

                case Status.Done:
                    nextStatus = Status.Done;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(currentAssignmentStatus.ToString());
            }

            return await _assignmentGateway.GetAssignmentStatusByStatusCode((int)nextStatus);
        }

        private Status MoveStatusBack() => Status.InWork;

        private async Task CopyCommentsFromParentAssignmentAsync(Assignment parent, long reproducedAssignmentId, string currentUserEmail)
        {
            if (parent.Comments is null)
            {
                return;
            }

            foreach (var comment in parent.Comments)
            {
                _ = await _commentService.CreateAsync(comment.Content, reproducedAssignmentId, currentUserEmail, comment.StatusCreated);
            }
        }

        private async Task CopyFilesFromParentAssignmentAsync(long assignmentId, long reproducedAssignmentId, string currentUserEmail)
        {
            List<AssignmentFile>? files = await _assignmentGateway.GetRelatedAssignmentsFilesWithData(assignmentId);
            if (files.Count is 0)
            {
                return;
            }

            foreach (var file in files)
            {
                _ = await _fileService.UploadFileToDbAsync(file.OriginName!, file.Content, reproducedAssignmentId, currentUserEmail);
            }
        }

        /// <summary>
        /// Определить есть ли у текущего пользователя права на добавление комментариев к поручению
        /// </summary>
        /// <param name="assignment">поручение</param>
        /// <param name="employee">текущий пользователь</param>
        private async Task<bool> HasPermissionToAddComment(Assignment assignment, Models.DTO.Common.Employee employee)
        {
            var hasPermissionToAddComment = false;

            //Получаю все должности текущего пользователя
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles = await _employeeService
                .GetAllEmployeePositionsByPositionId(employee.Id);

            if (employee.RoleId is 1 or 2 || currentEmployeeAllPositionsWithRoles.Values.Any(role => role is 1 or 2))
            {
                hasPermissionToAddComment = true;
            }
            else
            {
                if ((assignment.IsResponsibleExecutor(employee.Id)
                    || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == assignment.ResponsibleExecutorId))
                    && assignment.StatusId is (long)Status.InWork)
                {
                    hasPermissionToAddComment = true;
                }

                if ((assignment.IsResponsibleLeader(employee.Id)
                    || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == assignment.ResponsibleLeaderId))
                    && assignment.StatusId is (long)Status.InWork or (long)Status.Verified or (long)Status.Completed)
                {
                    hasPermissionToAddComment = true;
                }

                if ((assignment.IsResponsibleInspector(employee.Id)
                    || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == assignment.ResponsibleInspectorId))
                    && assignment.StatusId is (long)Status.Monitoring)
                {
                    hasPermissionToAddComment = true;
                }

                if ((assignment.IsAuthor(employee.Id)
                    || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == assignment.AuthorId))
                    && assignment.StatusId is (long)Status.Done)
                {
                    hasPermissionToAddComment = true;
                }
            }

            return hasPermissionToAddComment;
        }
    }
}