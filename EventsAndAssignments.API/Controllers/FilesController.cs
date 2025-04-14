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
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Загрузка файла (с привязкой в поручению)
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="assignmentId">Id поручения, к которому будет прикреплен файл</param>
        [HttpPost]
        [Route(nameof(UploadFile))]
        public async Task<ActionResult<UploadFileToDbResponseDto>> UploadFile([Required] IFormFile file,
            [Required][Range(1, long.MaxValue)] long assignmentId)
        {
            string? currentUserEmail = User.Claims
                .FirstOrDefault(c => c.Type is "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            string fileName = file.FileName;

            using MemoryStream stream = new();
            await file.CopyToAsync(stream);
            byte[] fileBody = stream.ToArray();

            UploadFileToDbResponseDto response =
                await _fileService.UploadFileToDbAsync(fileName, fileBody, assignmentId, currentUserEmail!);

            return Ok(response);
        }

        /// <summary>
        /// Скачивание файла из базы данных
        /// </summary>
        /// <param name="fileId">Id файла который мы хотим скачать</param>
        /// <returns>файл</returns>
        [HttpGet]
        [Route(nameof(DownloadFile))]
        public ActionResult DownloadFile([Required][Range(1, long.MaxValue)] long fileId)
        {
            DownloadFileResponse? loadedFile = _fileService.DownloadFile(fileId);

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

        /// <summary>
        /// Удаляет выбранный файл.
        /// </summary>
        /// <param name="fileId">Id удаленного файла.</param>
        /// <returns>возвращает dto модель к файлу</returns>
        [HttpPost]
        [Route(nameof(RemoveFile))]
        public async Task<ActionResult<RemoveFileDtoResponse>> RemoveFile([Required][Range(1, long.MaxValue)] long fileId)
        {
            string currentUserEmail = User.Claims
                .First(c => c.Type is "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            RemoveFileDtoResponse response =  await _fileService.RemoveFileAsync(fileId, currentUserEmail);

            return Ok(response);
        }
    }
}