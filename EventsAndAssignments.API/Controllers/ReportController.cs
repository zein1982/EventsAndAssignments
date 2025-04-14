using System.Globalization;
using System.IO.Compression;
using System.Security.Claims;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EventsAndAssignments.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        /// <summary>
        /// Получения архива с Excel файлами(отчеты по протоколам).
        /// </summary>
        /// <param name="reportResponseDto">Id протоколов по которым нужно получить отчет.</param>
        [HttpPost]
        [Route(nameof(GetExcelReportByProtocol))]
        public IActionResult GetExcelReportByProtocol(ExcelProtocolReportRequestDTO reportResponseDto)
        {
            if (reportResponseDto.Ids is null)
            {
                return BadRequest();
            }

            List<FileStreamResult> fileStreamResults = new();
            string protocolFolder = string.Empty;

            foreach (var id in reportResponseDto.Ids)
            {
                List<Assignment> dataForReport = _reportService.GetDataForExcelProtocolReport(id).ToList();

                if (dataForReport.IsNullOrEmpty())
                {
                    continue;
                }

                MemoryStream stream = _reportService.MakeReportByAssignments(dataForReport, reportResponseDto.TimeDifference);
                Protocol? protocol = dataForReport.FirstOrDefault()?.Protocol;

                FileStreamResult file = File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{protocol?.Name}.xlsx");

                if (reportResponseDto.Ids.Count <= 1)
                {
                    return file;
                }

                fileStreamResults.Add(file);

                if (string.IsNullOrEmpty(protocolFolder))
                {
                    protocolFolder = (protocol?.Folder is not null) ? protocol.Folder.Name : string.Empty;
                }
            }

            if (fileStreamResults.Count == 0)
            {
                return Ok(fileStreamResults);
            }

            return ArchiveFiles(fileStreamResults, protocolFolder);
        }

        /// <summary>
        /// Отчет по поручениям.
        /// </summary>
        [HttpPost]
        [Route(nameof(GetReportByAssignments))]
        public IActionResult GetReportByAssignments([FromBody] ByIdListRequest ids)
        {
            try
            {
                string currentUserEmail = "Rinat.Salimianov@evraz.com";//User.FindFirst(ClaimTypes.Email)?.Value
                                                                       //?? throw new ArgumentNullException(nameof(currentUserEmail));
                return File(_reportService.MakeReportByProtocol(ids.IdList).ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Отчет Все поручения");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// короткий отчет по поручениям.
        /// </summary>
        [HttpPost]
        [Route(nameof(GetShortAssignmentReport))]
        public IActionResult GetShortAssignmentReport([FromBody] ExcelProtocolReportRequestDTO requestParams)
        {
            try
            {
                string reportName =
                    $"Отчет по протоколу от  {DateTime.UtcNow + TimeSpan.FromHours(requestParams.TimeDifference):dd-MM-yyyy}.xlsx";

                return File(_reportService.MakeShortReportByAssignments(
                    requestParams.Ids!.ToList(), requestParams.TimeDifference).ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    reportName);
            }
            catch (Exception e)
            {
                _logger.LogError("Ошибка при формировании отчета. {ErrorMessage}", e.InnerException?.Message);
                return BadRequest(e.Message);
            }
        }

        private IActionResult ArchiveFiles(List<FileStreamResult> fileStreamResults, string archiveName)
        {
            MemoryStream memoryStream = new();
            using (ZipArchive zipArchive = new(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileStreamResult in fileStreamResults)
                {
                    ZipArchiveEntry entry = zipArchive.CreateEntry(fileStreamResult.FileDownloadName);
                    using Stream entryStream = entry.Open();
                    fileStreamResult.FileStream.CopyTo(entryStream);
                }
            }

            memoryStream.Position = 0;

            if (string.IsNullOrEmpty(archiveName))
            {
                archiveName = $"Выгрузка протоколов от { DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture) }";
            }

            return File(memoryStream, "application/7z", $"{archiveName}.7z");
        }
    }
}