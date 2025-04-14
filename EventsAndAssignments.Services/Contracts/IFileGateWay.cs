using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Contracts
{
    public interface IFileGateway
    {
        /// <summary>
        /// Скачивание файла
        /// </summary>
        /// <param name="fileId">Идентификатор файла</param>
        AssignmentFile? DownloadFile(long fileId);

        /// <summary>
        /// Загрузка файла в бд
        /// </summary>
        /// <param name="assignmentFile"></param>
        Task<AssignmentFile> UploadFileToDbAsync(AssignmentFile assignmentFile);
        Task<AssignmentFile?> RemoveFileAsync(long fileId);
    }
}