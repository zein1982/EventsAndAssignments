using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using Microsoft.EntityFrameworkCore;

namespace EventsAndAssignments.Db.Repositories
{
    public class InstructionFileGateWay : IInstructionFileGateWay
    {
        private readonly ApplicationDbContext _context;

        public InstructionFileGateWay(ApplicationDbContext context)
        {
            _context = context;
        }

        public InstructuonFile? DownloadInstructionFile(long fileId)
        {
            InstructuonFile? file = _context.InstructuonFiles.AsNoTracking()
                .FirstOrDefault(x => x.Id == fileId);

            return file;
        }

        public async Task<IReadOnlyCollection<InstructuonFile>> GetInstructionFileNamesAsync()
        {
            return await _context.InstructuonFiles
                .AsNoTracking()
                .Select(x => new InstructuonFile
                {
                    Id = x.Id,
                    SafetyName = x.SafetyName,
                    OriginName = x.OriginName,
                    Content = Array.Empty<byte>()
                })
                .ToListAsync();
        }

        public async Task<InstructuonFile> UploadFileToDbAsync(InstructuonFile instructionFile)
        {
            _context.InstructuonFiles.Add(instructionFile);
            await _context.SaveChangesAsync();
            return instructionFile;
        }
    }
}