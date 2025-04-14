using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Exceptions;
using EventsAndAssignments.Services.Interfaces;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace EventsAndAssignments.Services.Data
{
    public class FileService : IFileService
    {
        private readonly IFileGateway _fileGateway;
        private readonly IAssignmentHistoryService _assignmentHistoryService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly ILogger<FileService> _logger;

        public FileService(IFileGateway fileGateWay,
            IAssignmentHistoryService assignmentHistoryService,
            IEmployeeService employeeService,
            IMapper mapper,
            ILogger<FileService> logger)
        {
            _fileGateway = fileGateWay;
            _assignmentHistoryService = assignmentHistoryService;
            _employeeService = employeeService;
            _mapper = mapper;
            _logger = logger;
        }

        public DownloadFileResponse? DownloadFile(long fileId)
        {
            AssignmentFile? downloaded = _fileGateway.DownloadFile(fileId);

            return _mapper.Map<DownloadFileResponse>(downloaded!);
        }

        public async Task<RemoveFileDtoResponse> RemoveFileAsync(long id, string currentUserEmail)
        {
            Models.DTO.Common.Employee? currentEmployee =
                _employeeService.GetEmployeeByEmail(currentUserEmail) ?? throw new EntityNotFoundException();

            AssignmentFile? removed = await _fileGateway.RemoveFileAsync(id)
                ?? throw new EntityNotFoundException(id);

            await _assignmentHistoryService.CreateFromAssignmentFilesModificationAsync(removed, FileAction.Remove, currentEmployee);

            return _mapper.Map<RemoveFileDtoResponse>(removed);
        }

        public async Task<UploadFileToDbResponseDto> UploadFileToDbAsync(
            string fileName,
            byte[] fileBody,
            long assignmentId,
            string currentUserEmail)
        {
            Models.DTO.Common.Employee currentEmployee =
                _employeeService.GetEmployeeByEmail(currentUserEmail) ?? throw new EntityNotFoundException();

            string size = GetFormattedFileSizeString(fileBody.Length); //TODO в рамках общей миграции изменить наименование поля с SafetyName на SafetyName
            AssignmentFile assignmentFile = new()
            {
                OriginName = fileName,
                SafetyName = size,
                Content = fileBody,
                Created = DateTime.UtcNow,
                CreatedBy = currentEmployee.Id,
                Updated = DateTime.UtcNow,
                UpdatedBy = currentEmployee.Id,
                AssignmentId = assignmentId
            };

            //Запись файла в БД
            AssignmentFile newFile =  await _fileGateway.UploadFileToDbAsync(assignmentFile);

            _logger.LogInformation("Файл с именем {fileName} загружен в бд.",
            newFile.OriginName);

            //Записываю в историю
            await _assignmentHistoryService
                .CreateFromAssignmentFilesModificationAsync(newFile, FileAction.Add, currentEmployee);

            return _mapper.Map<UploadFileToDbResponseDto>(newFile);
        }

        private string GetFormattedFileSizeString(int fileSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (fileSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                fileSize /= 1000;
            }

            string result = $" ({fileSize:0.##} {sizes[order]})";

            return result;
        }
    }
}