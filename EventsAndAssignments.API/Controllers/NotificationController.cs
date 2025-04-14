using System.ComponentModel.DataAnnotations;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notifiService;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService service, ILogger<NotificationController> logger, IMapper mapper)
        {
            _notifiService = service;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Количество новых уведомлений для пользователя //Не реализованно
        /// </summary>
        /// <param name="email">Email пользователя</param>
        /// <returns>количество уведомлений для пользователя</returns>
        [HttpGet]
        [Route(nameof(GetNotificationCount))]
        public ActionResult<int> GetNotificationCount([Required] string email)
        {
            //Не реализованно

            return Ok(-1);
        }

        ///// <summary>
        ///// Уведомления для текущего пользователя //Не реализованно
        ///// </summary>
        ///// <returns>Уведомления для текущего пользователя</returns>
        //[HttpGet]
        //[Route(nameof(GetNotifications))]
        //public ActionResult<List<Notification>> GetNotifications()
        //{
        //    //Не реализованно
        //    List<Notification> ret= new();

        //    return Ok(ret);
        //}

        /// <summary>
        /// Настройки уведомлений для текущего пользователя
        /// </summary>
        /// <returns>Настройки уведомлений для текущего пользователя</returns>
        [HttpGet]
        [Route(nameof(GetNotificationSettings))]
        public async Task<ActionResult<NotificationSettingResponseDTO>> GetNotificationSettings()
        {
            string userEmail = User.Claims.First(c => c.Type is "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            return Ok(await _notifiService.GetNotificationSettingsAsync(userEmail));
        }

        /// <summary>
        /// Установка настроек для текущего пользователя
        /// </summary>
        /// <returns>Результат </returns>
        [HttpPost]
        [Route(nameof(SetNotificationSettings))]
        public async Task<ActionResult<bool>> SetNotificationSettings([FromBody][Required] NotificationSettingRequestDTO request)
        {
            string userEmail = User.Claims
                .First(c => c.Type is "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            bool result = await _notifiService.SetNotificationSettingsAsync(userEmail, request);

            return Ok(result);
        }

        /// <summary>
        /// Разослать уведомления по выбранным поручениям (НЕ РЕАЛИЗОВАНО)
        /// </summary>
        /// <param name="assignmentsIds">Список идентификаторов поручений</param>
        [HttpPost]
        [Route(nameof(SendNotificationsOnAssignments))]
        public async Task<ActionResult> SendNotificationsOnAssignments([Required][FromBody] ICollection<long> assignmentsIds)
        {
            await Task.CompletedTask;

            return Ok();
        }
    }
}