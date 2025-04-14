using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using Microsoft.EntityFrameworkCore;

namespace EventsAndAssignments.Db.Repositories
{
    public class FileGateway : IFileGateway
    {
        private readonly ApplicationDbContext _context;

        public FileGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public AssignmentFile? DownloadFile(long fileId)
        {
            AssignmentFile? file =  _context.Files
                .NotRemoved()
                .FirstOrDefault(file => file.Id == fileId);

            return file;
        }

        public async Task<AssignmentFile?> RemoveFileAsync(long fileId)
        {
            AssignmentFile removed = await _context.Files.SingleAsync(x=>x.Id==fileId);
            removed.Removed = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return removed;
        }

        public async Task<AssignmentFile> UploadFileToDbAsync(AssignmentFile assignmentFile)
        {
            _context.Files.Add(assignmentFile);
            await _context.SaveChangesAsync();
            return assignmentFile;
        }
    }
}