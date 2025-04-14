using EventsAndAssignments.Models.DTO.Common;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Sorts;

namespace EventsAndAssignments.Services.Interfaces
{
    /// <summary>
    /// Сервис обслуживающий папки протоколов
    /// </summary>
    public interface IProtocolFoldersService
    {
        /// <summary>
        /// Создать папку протокола в источнике данных
        /// </summary>
        Task<bool> CreateProtocolFolderAsync(string folderName, string userMail, ICollection<Guid> allowedEmployeesIds);

        /// <summary>
        /// Получить папки из источника данных
        /// </summary>
        Task<(IReadOnlyCollection<ProtocolFolder>, int count)> GetProtocolFoldersAsync(RequestParams filter, string userMail);

        /// <summary>
        /// Возвращает попку по ID
        /// </summary>
        /// <param name="id"></param>
        Task<ProtocolFolder> GetProtocolFolderAsync(long id);

        /// <summary>
        /// Обновить папку протокола в источнике данных
        /// </summary>
        Task<bool> UpdateProtocolFolderAsync(long id, string folderName, Guid folderOwner, ICollection<Guid> allowedEmployeesIds, string userMail);

        /// <summary>
        /// Удалить (пометить, как удаленную или архивировать) папку протокола
        /// </summary>
        /// <param name="idsList">Список Id которые нужно удалить</param>
        Task<IReadOnlyCollection<ProtocolFolder>> RemoveProtocolFolderAsync(IReadOnlyCollection<long> idsList);

        /// <summary>
        /// Заархивировать (пометить только для чтения) папку протокола
        /// </summary>
        Task ArchiveProtocolFolderAsync(List<long> idList);

        /// <summary>
        /// Получить пользователей имеющих доступ к папке
        /// </summary>
        /// <param name="folderId">Идентификатор папки</param>
        public Task<IReadOnlyCollection<Employee>> GetEmployeesAllowedToFolder(long folderId);

        /// <summary>
        /// Добавить пользователя с идентификатором <see cref="employeeId"/> в список пользователей имеющик доступ к папке <see cref="folderId"/>
        /// </summary>
        /// <param name="folderId">Идентификатор папки</param>
        /// <param name="employeeId">Идентификатор пользователя, которого нужно добавить в список</param>
        /// <param name="currentEmployeeMail">Email адрес текущего пользователя системы</param>
        Task<bool> AddAllowedEmployeeAsync(long folderId, Guid employeeId, string currentEmployeeMail);

        /// <summary>
        /// Удалить пользователя с идентификатором <see cref="employeeId"/> в список пользователей имеющик доступ к папке <see cref="folderId"/>
        /// </summary>
        /// <param name="folderId">Идентификатор папки</param>
        /// <param name="employeeId">Идентификатор пользователя, которого нужно добавить в список</param>
        /// <param name="currentEmployeeMail">Email адрес текущего пользователя системы</param>
        Task<bool> RemoveAllowedEmployeeAsync(long folderId, Guid employeeId, string currentEmployeeMail);
    }
}