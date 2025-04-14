using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Sorts;

namespace EventsAndAssignments.Services.Contracts
{
    /// <summary>
    /// Шлюз поручений и источников данных
    /// </summary>
    public interface IAssignmentsGateway
    {
        /// <summary>
        /// Получить поручение по Id (с подгруженными файлами)
        /// </summary>
        /// <param name="id"></param>
        Task<Assignment?> GetAssignmentByIdWithFilesAsync(long id);

        /// <summary>
        /// Получить поручение по Id (Оптимизировано по количеству включенных свойств)
        /// </summary>
        /// <param name="id"></param>
        Task<Assignment?> GetAssignmentByIdAsync(long id);

        /// <summary>
        /// Получить поручение по Id группы и версии
        /// </summary>
        /// <param name="groupId">Id группы</param>
        /// <param name="version">Версия</param>
        /// <param name="subversion">Под версия</param>
        Task<Assignment?> GetAssignmentByGroupIdAndVersionAsync(long groupId, int version, int subversion);

        /// <summary>
        /// Получить список поручений принадлежащих одной группе (размноженные)
        /// </summary>
        /// <param name="groupId">Id группы поручений</param>
        Task<ICollection<Assignment>> GetAssignmentsByGroupId(long groupId);

        /// <summary>
        /// Получить общее количество поручений соответствующих условию
        /// </summary>
        /// <param name="predicate">Условие</param>
        Task<int> GetAssignmentCountAsync(Func<Assignment, bool>? predicate = null);

        /// <summary>
        /// Получить количество поручений в протоколе
        /// </summary>
        /// <param name="protocolId">Id протокола</param>
        public long GetAssignmentsCountInProtocol(long protocolId);

        /// <summary>
        /// Создать новое поручение
        /// </summary>
        /// <param name="assignment">Поручение</param>
        Task<Assignment> CreateAssignmentAsync(Assignment assignment);

        /// <summary>
        /// Обновить поручение
        /// </summary>
        /// <param name="assignment">Поручение</param>
        Task<Assignment> UpdateAssignmentAsync(Assignment assignment);

        /// <summary>
        /// Удалить поручение
        /// </summary>
        /// <param name="id">Id поручения</param>
        Task RemoveAssignmentsAsync(IReadOnlyCollection<long> id);

        /// <summary>
        /// Получить список поручений согласно фильтрам и роли текущего пользователя
        /// </summary>
        /// <param name="filterParam">фильтра</param>
        /// <param name="currentEmployeeAllPositionsWithRoles">список всех должностей текущего пользователя с ролями</param>
        /// <param name="noPagination">разбить на страницы</param>
        (List<Assignment> items, int count) GetFilteredAssignments(RequestParams filterParam, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles, bool noPagination = false);

        /// <summary>
        /// Получить список поручений согласно фильтрам и роли текущего пользователя
        /// </summary>
        /// <param name="filterParam">фильтра</param>
        /// <param name="currentEmployeeAllPositionsWithRoles">список всех должностей текущего пользователя с ролями</param>
        /// <param name="noPagination">разбить на страницы</param>
        List<long> GetFilteredAssignmentsIds(RequestParams filterParam, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles);

        /// <summary>
        /// Получить поручения для отчета
        /// </summary>
        /// <param name="protocolId">Номер протокола, которому принадлежат поручения</param>
        IReadOnlyCollection<Assignment> GetAssignmentForExcelReport(long protocolId);

        /// <summary>
        /// Получить список возможных статусов поручения
        /// </summary>
        /// <param name="hasResponsibleInspector">Присутствует ли в поручении контролер</param>
        Task<ICollection<AssignmentStatus>> GetAllAssignmentStatusesAsync(bool hasResponsibleInspector);

        /// <summary>
        /// Получить статус поручения по коду
        /// </summary>
        /// <param name="statusCode">Код статуса</param>
        Task<AssignmentStatus> GetAssignmentStatusByStatusCode(int statusCode);

        /// <summary>
        /// Получить поручения для короткого отчета
        /// </summary>
        /// <param name="ids">Список Id поручений</param>
        IReadOnlyCollection<Assignment> GetAssignmentsForShortReport(List<long> ids);

        /// <summary>
        /// Переименовать поручения в протоколе и отсортировать по имени
        /// </summary>
        /// <param name="protocolId">Номер протокола</param>
        Task RenameAndOrderAllAssignmentsInProtocol(long protocolId);

        /// <summary>
        /// Получить общее количество находящихся в работе поручений для сотрудника и всех его должностей
        /// </summary>
        /// <param name="positionId">Id сотрудника</param>
        /// <param name="currentEmployeeAllPositionsWithRoles">Список должностей сотрудника с ролями</param>
        Task<int> GetEmployeeAssignmentCount(Guid positionId, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles);

        /// <summary>
        /// Получить количество просроченных находящихся в работе поручений для сотрудника и всех его должностей
        /// </summary>
        /// <param name="positionId">Id сотрудника</param>
        /// <param name="currentEmployeeAllPositionsWithRoles">Список должностей сотрудника с ролями</param>
        Task<int> GetEmployeeAssignmentUnFinishedCount(Guid positionId, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles);

        /// <summary>
        /// Получить список прикрепленных к поручению файлов с ответственным
        /// </summary>
        /// <param name="assignmentId">Id поручения</param>
        Task<List<AssignmentFile>> GetRelatedAssignmentsFilesWithData(long assignmentId);

        /// <summary>
        /// Получить список сотрудников с ролью администратор имеющих доступ к папке
        /// </summary>
        /// <param name="id">Идентификатор поручения принадлежащего папке</param>
        List<Guid> GetFolderAllowedEmployees(long id);
    }
}