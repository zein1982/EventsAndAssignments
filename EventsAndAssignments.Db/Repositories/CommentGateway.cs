using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EventsAndAssignments.Db.Repositories
{
    public class CommentGateway : ICommentGateway
    {
        private readonly ApplicationDbContext _context;

        public CommentGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            EntityEntry<Comment> created = await _context.AddAsync(comment);
            await _context.SaveChangesAsync();

            await created
                .Reference(e => e.CreatedByNavigation)
                .LoadAsync();

            return created.Entity;
        }

        public async Task<Comment?> GetLastAsync(long assignmentId)
        {
            Comment? last = await _context.Comments
                .NotRemoved()
                .Include(e => e.CreatedByNavigation)
                .Where(e => e.AssignmentId == assignmentId)
                .OrderBy(e => e.Id)
                .LastOrDefaultAsync();

            return last;
        }

        public async Task<IReadOnlyCollection<Comment>> GetAllCommentsForAssignmentAsync(long? assignmentId)
        {
            return await _context
                .Comments
                .NotRemoved()
                .Include(e => e.CreatedByNavigation)
                .Where(comment => comment.AssignmentId == assignmentId)
                .OrderBy(comment => comment.Created)
                .ToListAsync();
        }

        public async Task<long> RemoveCommentAsync(long id)
        {
            Comment comment = await _context.Comments.FirstAsync(x => x.Id == id);

            comment.Removed = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return comment.Id;
        }

        public async Task UpdateComment(long id, string newText)
        {
            Comment updated = await _context.Comments.FirstAsync(x => x.Id == id);
            updated.Content = newText;
            await _context.SaveChangesAsync();
        }
    }
}