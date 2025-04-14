using EventsAndAssignments.Services.Sorts;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface IFilterService
    {
        Task<IReadOnlyCollection<FieldFilter>> GetAssignmentFilters();
        IReadOnlyCollection<FieldFilter> GetFolderFilters();
        IReadOnlyCollection<FieldFilter> GetProtocolFilters();
        IReadOnlyCollection<string> GetSortsAssignmentName();
    }
}