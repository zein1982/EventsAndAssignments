using EventsAndAssignments.Services.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EventsAndAssignments.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PingController : ControllerBase
    {
        private readonly ILogger<PingController> _log;
        private readonly IOptions<MailOptions> _options;
        private readonly IOptions<NotificationsOptions> _notifications;

        public PingController(
            ILogger<PingController> logger,
            IOptions<MailOptions> options,
            IOptions<NotificationsOptions> notifications)
        {
            _log = logger;
            _options = options;
            _notifications = notifications;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            try
            {
                string userName = User.Claims.First(c => c.Type is "name").Value;
                _log.LogInformation($"Ping from {userName}");
                _log.LogInformation("Переменная Host: {Host} MAIL__PORT: {Mail} MAIL__USER: {User}, fronturl: {front}, pas {pas}",
                    _options.Value.Host, _options.Value.Port,
                    _options.Value.User, _notifications.Value.FrontUrl, _options.Value.Pass);

                return Ok(_notifications.Value.FrontUrl);
            }
            catch (Exception e)
            {
                _log.LogCritical(e.Message);
                return BadRequest(e.Message);
            }
        }
    }
}