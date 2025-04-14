using Microsoft.AspNetCore.Mvc;
using EventsAndAssignments_DataTransfer.Services;

namespace EventsAndAssignments_DataTransfer.Controllers
{
    /// <summary>
    /// API-контроллер сервиса
    /// </summary>
    [ApiController]
    public class TransferServiceController : ControllerBase
    {
        private readonly DbTransferServiceControl _serviceControl;

        /// <inheritdoc cref="TransferServiceController"/>
        public TransferServiceController(DbTransferServiceControl serviceControl)
        {
            _serviceControl = serviceControl;
        }

        /// <summary>
        /// Получить лог работы сервиса
        /// </summary>
        [HttpGet]
        [Route(nameof(GetServiceLog))]
        public ActionResult GetServiceLog()
        {
            var serviceUsageLog = _serviceControl.GetServiceLogDelegate!();

            return Ok(new { ActivityLog = serviceUsageLog });
        }

        /// <summary>
        /// Индикатор работы сервиса в данный момент
        /// </summary>
        [HttpGet]
        [Route(nameof(GetActivityStatus))]
        public ActionResult GetActivityStatus()
        {
            bool serviceActive = _serviceControl.GetServiceActivityStatusDelegate!();
            string message = serviceActive ? "Service active" : "Service is not active";

            return Ok(new { Message = message });
        }

        /// <summary>
        /// Приостановить работу сервиса
        /// </summary>
        /// <param name="comment">Комментарий о приостановке работы сервиса</param>
        [HttpPost]
        [Route(nameof(SuspendService))]
        public ActionResult SuspendService(string? comment = null)
        {
            _serviceControl.SuspendServiceDelegate!(comment);

            return Ok(new { Message = "Service suspended" });
        }

        /// <summary>
        /// Продолжить работу сервиса (если она была приостановлена)
        /// </summary>
        /// <param name="comment">Комментарий о возобновлении работы сервиса</param>
        [HttpPost]
        [Route(nameof(ContinueService))]
        public ActionResult ContinueService(string? comment = null)
        {
            _serviceControl.ContinueServiceDelegate!(comment);

            return Ok(new { Message = "Service continued" });
        }
    }
}