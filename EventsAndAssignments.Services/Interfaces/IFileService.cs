using EventsAndAssignments.Models.DTO.Response;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface IFileService
    {
        DownloadFileResponse? DownloadFile(long fileId);

        Task<UploadFileToDbResponseDto> UploadFileToDbAsync(
            string fileName, byte[] fileBody, long assignmentId, string currentEmployee);

        Task<RemoveFileDtoResponse> RemoveFileAsync(long id, string currentEmployee);
    }
}