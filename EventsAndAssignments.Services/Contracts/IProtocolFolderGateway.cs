using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Sorts;

namespace EventsAndAssignments.Services.Contracts
{
    /// <summary>
    /// Шлюз папок протоколов с источником данных
    /// </summary>
    public interface IProtocolFoldersGateway
    {
        Task<bool> CreateProtocolFolderAsync(string folderName, Guid currentEmployeeId, long currentEmployeeRoleId, ICollection<Guid> allowedEmployeesIds);
        Task<ProtocolFolder> GetProtocolFolderAsync(long id);
        Task<(IReadOnlyCollection<ProtocolFolder>, int count)> GetProtocolFoldersAsync(RequestParams filter, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles);

        Task<bool> UpdateProtocolFolderAsync(long id, string folderName, Guid folderOwner, Guid currentEmployeeId, long currentEmployeeRoleId,
            ICollection<Guid> allowedEmployeesIds, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles);

        Task ArchiveProtocolFolderAsync(List<long> idList);
        Task<IReadOnlyCollection<ProtocolFolder>> RemoveProtocolFolderAsync(IReadOnlyCollection<long> idList);
        Task<string> GetFolderName(long folderId);
        Task<IReadOnlyCollection<Employee>> GetEmployeesAllowedToFolderAsync(long folderId);
        Task<bool> AddAllowedEmployeeAsync(long folderId, Guid employeeToAdd, Guid currentEmployeeId, long currentEmployeeRoleId);
        Task<bool> RemoveAllowedEmployeeAsync(long folderId, Guid employeeToRemove, Guid currentEmployeeId, long currentEmployeeRoleId);
    }
}