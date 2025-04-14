using System.Linq.Expressions;
using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Contracts
{
    public interface IEmployeeGateway
    {
        /// <summary>
        /// Вернуть трудозанятых в соответствии с фильтрами
        /// </summary>
        /// <param name="userName">Фильтр по ФИО</param>
        /// <param name="count">Максимальное количетсов возвращаемых результатов</param>
        IReadOnlyCollection<Employee> GetEmployees(string? userName = null, int count = 50);

        /// <summary>
        /// Вернуть трудозанятого, соответствующего переданному предикату
        /// </summary>
        /// <param name="predicate">Предикат соответствущий запросу</param>
        Employee? GetEmployeeByPredicate(Expression<Func<Employee, bool>> predicate);

        /// <summary>
        /// Получить фотографию пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="isSmall">Тип фото (полный или уменьшенный вариант)</param>
        Task<byte[]?> GetEmployeePhotoById(Guid id, bool isSmall);

        /// <summary>
        /// Задать роль для пользователя
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <param name="roleId">Id роли</param>
        Task<Employee> SetEmployeeRole(Guid id, long roleId);

        /// <summary>
        /// Получить идентификаторы всех должностей сотрудника по идентификатору одной из них
        /// </summary>
        /// <param name="positionId">Идентификатор должности сотрудника</param>
        /// <returns>Список, только для чтения, с идентификаторами всех должностей сотрудника</returns>
        Task<IReadOnlyDictionary<Guid, long?>> GetAllEmployeePositionsByPositionId(Guid positionId);

        /// <summary>
        /// Получить идентификаторы всех должностей сотрудника по идентификатору сотрудника
        /// </summary>
        /// <param name="employeeId">Идентификатор сотрудника</param>
        /// <returns>Словарь, только для чтения, с идентификаторами всех должностей сотрудника и ролью для каждой должности</returns>
        Task<IReadOnlyDictionary<Guid, long?>> GetAllEmployeePositionsByEmployeeId(Guid employeeId);
    }
}