using EventsAndAssignments.Models.DTO.Response;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponseDto?> GetLastAsync(long assignmentId);
        Task<ICollection<CommentResponseDto>> GetAllCommentsForAssignmentAsync(long assignmentId, string employeeEmail);
        Task<CommentResponseDto> CreateAsync(string? content, long? assignmentId, string currentUserEmail, long? assignmentStatus);
        Task<long> RemoveCommentAsync(long commentId);
        Task UpdateCommentAsync(long commentId, string newText);
    }
}