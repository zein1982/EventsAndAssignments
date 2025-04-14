using System.ComponentModel.DataAnnotations;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class InstructionFileController : ControllerBase
    {
        private readonly IFileInstructionService _fileInstructionService;

        public InstructionFileController(
            IFileInstructionService fileInstructionService)
        {
            _fileInstructionService = fileInstructionService;
        }

        [Route(nameof(UploadInstructionFile))]
        [HttpPost]
        public async Task<ActionResult<UploadFileToDbResponseDto>> UploadInstructionFile([Required] IFormFile file)
        {
            string fileName = file.FileName;

            using MemoryStream stream = new();
            await file.CopyToAsync(stream);
            byte[] fileBody = stream.ToArray();
            UploadFileToDbResponseDto response =
            await _fileInstructionService.UploadFileToDbAsync(fileName, fileBody);

            return Ok(response);
        }

        [HttpGet]
        [Route(nameof(DownloadInstructionFile))]
        public ActionResult DownloadInstructionFile([Required][Range(1, long.MaxValue)] long fileId)
        {
            DownloadFileResponse? loadedFile = _fileInstructionService.DownloadFile(fileId);

            if (loadedFile is null)
            {
                return NotFound();
            }

            FileContentResult result = File(
                loadedFile.Content,
                "application/octet-stream",
                loadedFile.OriginName);

            return result;
        }

        [HttpGet]
        [Route(nameof(GetInstructionFileNames))]
        public async Task<ActionResult<IReadOnlyCollection<FileNameResponseDto>>> GetInstructionFileNames()
        {
            return Ok(await _fileInstructionService.GetFileInstructionNames());
        }
    }
}