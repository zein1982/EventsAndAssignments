using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Interfaces;
using MapsterMapper;

namespace EventsAndAssignments.Services.Data
{
    public class FileInstructionService : IFileInstructionService
    {
        private readonly IInstructionFileGateWay _instructionFileGateWay;
        private readonly IMapper _mapper;

        public FileInstructionService(
            IInstructionFileGateWay instructionFileGateWay,
            IMapper mapper)
        {
            _instructionFileGateWay = instructionFileGateWay;
            _mapper = mapper;
        }

        public DownloadFileResponse? DownloadFile(long fileId)
        {
            InstructuonFile? downloaded = _instructionFileGateWay.DownloadInstructionFile(fileId);

            return _mapper.Map<DownloadFileResponse?>(downloaded!);
        }

        public async Task<IReadOnlyCollection<FileNameResponseDto>> GetFileInstructionNames()
        {
            IReadOnlyCollection<InstructuonFile> response =  await _instructionFileGateWay.GetInstructionFileNamesAsync();
            return _mapper.Map<IReadOnlyCollection<FileNameResponseDto>>(response);
        }

        public async Task<UploadFileToDbResponseDto> UploadFileToDbAsync(string fileName, byte[] fileBody)
        {
            string size = GetFormattedFileSizeString(fileBody.Length); //TODO в рамках общей миграции изменить наименование поля с SafetyName на SafetyName
            InstructuonFile instructuonFile = new()
            {
                OriginName = fileName,
                SafetyName = size,
                Content = fileBody
            };

            //Запись файла в БД
            InstructuonFile newFile =  await _instructionFileGateWay.UploadFileToDbAsync(instructuonFile);

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