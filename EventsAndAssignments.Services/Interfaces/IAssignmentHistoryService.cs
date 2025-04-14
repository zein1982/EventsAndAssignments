using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.DTO_GottaGetOutOfHere;
using EventsAndAssignments.Services.Enums;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface IAssignmentHistoryService
    {
        public Task CreateFromAssignmentModificationAsync(
            Assignment from,
            Assignment to,
            Models.DTO.Common.Employee currentEmployee);

        public Task<AssignmentHistoryMessage> CreateFromAssignmentFilesModificationAsync(
            AssignmentFile file,
            FileAction action,
            Models.DTO.Common.Employee currentEmployee);

        public Task<ICollection<AssignmentHistoryResponseDto>> GetAll(long assignmentId);
    }
}