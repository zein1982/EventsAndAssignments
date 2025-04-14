using EventsAndAssignments.Models.DTO;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Получить трудозанятого по его email
        /// </summary>
        /// <param name="userEmail">Email трудозанятого</param>
        Models.DTO.Common.Employee? GetEmployeeByEmail(string userEmail);

        /// <summary>
        /// Получить трудозанятого со всеми должностями по его email
        /// </summary>
        /// <param name="userEmail">Email трудозанятого</param>
        Task<EmployeeWithAllPositionsDto?> GetEmployeeWithAllPositionsByEmail(string userEmail);

        /// <summary>
        /// Получить трудозанятых по имени
        /// </summary>
        IReadOnlyCollection<Models.DTO.Common.Employee> GetEmployeesByName(string name, int count);

        /// <summary>
        /// Получить трудозанятого по его id
        /// </summary>
        Models.DTO.Common.Employee? GetEmployeeById(Guid id);

        /// <summary>
        /// Получить фотографию трудозанятого по идентификатору
        /// </summary>
        /// <param name="id">Идентиификатор трудозанятого</param>
        /// <param name="isSmall">Тип фото (маленькая или большая)</param>
        Task<byte[]?> GetEmployeePhotoById(Guid id, bool isSmall);

        /// <summary>
        /// Задать роль для трудозанятого
        /// </summary>
        /// <param name="employeeId">Id трудозанятого</param>
        /// <param name="roleId">Id роли</param>
        Task<Models.DTO.Common.Employee> SetEmployeeRole(Guid employeeId, long roleId);

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