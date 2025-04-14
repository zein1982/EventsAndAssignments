using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Contracts
{
    public interface IAssignmentHistoryGateway
    {
        Task<AssignmentHistory> CreateAsync(AssignmentHistory historyRecord);
        Task<ICollection<AssignmentHistory>> GetAllAsync(long? assignmentId);
    }
}