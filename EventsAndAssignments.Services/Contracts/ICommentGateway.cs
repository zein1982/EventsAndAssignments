using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Contracts
{
    public interface ICommentGateway
    {
        Task<Comment?> GetLastAsync(long assignmentId);
        Task<IReadOnlyCollection<Comment>> GetAllCommentsForAssignmentAsync(long? assignmentId);
        Task<Comment> CreateAsync(Comment comment);
        Task<long> RemoveCommentAsync(long id);
        Task UpdateComment(long id, string newText);
    }
}