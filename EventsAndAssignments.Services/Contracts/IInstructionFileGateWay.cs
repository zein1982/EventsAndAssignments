using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Contracts
{
    public interface IInstructionFileGateWay
    {
        InstructuonFile? DownloadInstructionFile(long fileId);

        /// <summary>
        /// Загрузка файла в бд
        /// </summary>
        /// <param name="assignmentFile"></param>
        Task<InstructuonFile> UploadFileToDbAsync(InstructuonFile assignmentFile);
        Task<IReadOnlyCollection<InstructuonFile>> GetInstructionFileNamesAsync();
    }
}