using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EventsAndAssignments.API.Authentication;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Interfaces;
using EventsAndAssignments.Services.Sorts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProtocolsController : ControllerBase
    {
        private readonly IProtocolService _protocolService;

        public ProtocolsController(IProtocolService service)
        {
            _protocolService = service;
        }

        /// <summary>
        /// Возвращает список протоколов
        /// </summary>
        [HttpPost]
        [Route(nameof(GetProtocols))]
        [HasPermission(Permission.ReadProtocol)]
        public async Task<ActionResult<IEnumerable<ProtocolFolderRequestDTO>>> GetProtocols([FromBody] RequestParams? filter = null)
        {
            string userEmail = User.Claims
                .First(c => c.Type is "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            (ICollection<ProtocolResponseDTO>? protocols, int protocolsCount) = await _protocolService.GetAllAsync(filter!, userEmail);
            ListResponseDTO<ProtocolResponseDTO> response = new()
            {
                Count = protocolsCount,
                Page = filter.Page,
                PageCount = filter.Count,
                Items = protocols.ToList()
            };
            return Ok(response);
        }

        /// <summary>
        /// Поучение данных для короткого отчета по протоколу
        /// </summary>
        /// <param name="protocolId">The identifier.</param>
        [HttpGet]
        [Route(nameof(GetShortProtocolReport))]
        [HasPermission(Permission.ReadProtocol)]
        public async Task<ActionResult<IReadOnlyCollection<ShortProtocolReportResponseDto>>> GetShortProtocolReport(
            [Required][Range(1, long.MaxValue, ErrorMessage = "Недопустимое значение идентификатора")] long protocolId)
        {
            return Ok(await _protocolService.GetShortReportData(protocolId));
        }

        /// <summary>
        /// Создание протокола
        /// </summary>
        /// <param name="protocolDto">Данные о создателе и дате создания</param>
        /// <returns>Созданный протокол</returns>
        [HttpPost]
        [Route(nameof(CreateProtocol))]
        [HasPermission(Permission.CreateProtocol)]
        public async Task<ActionResult<CreateProtocolResponseDTO>> CreateProtocol([Required][FromBody] CreateProtocolRequestDTO protocolDto)
        {
            //Получаю текущего пользователя
            string currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new ArgumentNullException(nameof(currentUserEmail));

            CreateProtocolResponseDTO response = await _protocolService.CreateAsync(protocolDto, currentUserEmail);

            return Ok(response);
        }

        /// <summary>
        /// Обновление протокола.
        /// </summary>
        /// <param name="protocoldto">The protocoldto.</param>
        /// <returns>CreateProtocolResponseDTO</returns>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/Protocols/UpdateProtocol
        ///     {
        ///        "Id": 10,
        ///        "Name": "Новое имя"
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [Route(nameof(UpdateProtocol))]
        [HasPermission(Permission.UpdateProtocol)]
        public async Task<ActionResult<CreateProtocolResponseDTO>> UpdateProtocol([Required][FromBody] UpdateProtocolRequestDTO protocoldto)
        {
            string userEmail = User.Claims
                .First(c => c.Type is "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            return Ok(await _protocolService.UpdateProtocolAsync(protocoldto.Id, protocoldto.Name, userEmail));
        }

        /// <summary>
        /// Удаляет протокол
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/Protocols/RemoveProtocol
        ///     {
        ///        "description": "причина удаления",
        ///        "itemsToRemove": "[2, 10]"
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [Route(nameof(RemoveProtocol))]
        [HasPermission(Permission.RemoveProtocol)]
        public async Task<ActionResult<IReadOnlyCollection<CreateProtocolResponseDTO>>> RemoveProtocol([Required] RemoveRequest<long> removeRequest)
        {
            string userEmail = User.Claims
                .First(c => c.Type is "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            ICollection<CreateProtocolResponseDTO> response = await _protocolService.RemoveProtocolsByAdmin(removeRequest.ItemsToRemove!, removeRequest.Description, userEmail);
            return Ok(response);
        }
    }
}