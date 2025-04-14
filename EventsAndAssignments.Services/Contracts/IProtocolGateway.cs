using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Sorts;

namespace EventsAndAssignments.Services.Contracts
{
    public interface IProtocolGateway
    {
        public Task<IReadOnlyCollection<Assignment>> GetShortReportData(long id);
        public IReadOnlyCollection<Assignment> GetDataForByProtocolReport(List<long> ids);
        public Task<int> GetProtocolCountInFolder(long folderId);
        public (ICollection<Protocol> items, int count) GetAll(RequestParams filterParams, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles);
        public Task<Protocol> CreateAsync(Protocol name);
        public Task<Protocol> UpdateProtocolAsync(long protocolId, string name, Guid currentUserPositionId);
        public Task<ICollection<Protocol>> ArchiveProtocol(IReadOnlyCollection<long> ids);
    }
}