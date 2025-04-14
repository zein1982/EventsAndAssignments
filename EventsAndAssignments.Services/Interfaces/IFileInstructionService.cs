using EventsAndAssignments.Models.DTO.Response;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface IFileInstructionService
    {
        DownloadFileResponse? DownloadFile(long fileId);

        Task<UploadFileToDbResponseDto> UploadFileToDbAsync(
            string fileName, byte[] fileBody);

        Task<IReadOnlyCollection<FileNameResponseDto>> GetFileInstructionNames();
    }
}