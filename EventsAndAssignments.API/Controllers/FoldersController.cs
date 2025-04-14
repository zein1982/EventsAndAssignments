using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EventsAndAssignments.API.Authentication;
using EventsAndAssignments.Models.DTO.Common;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Interfaces;
using EventsAndAssignments.Services.Sorts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FoldersController : ControllerBase
    {
        private readonly IProtocolFoldersService _folderService;
        private readonly ILogger<FoldersController> _logger;

        public FoldersController(
            IProtocolFoldersService service,
            ILogger<FoldersController> logger)
        {
            _folderService = service;
            _logger = logger;
        }

        /// <summary>
        /// Создает папку протоколов
        /// </summary>
        [HttpPost]
        [Route(nameof(CreateFolder))]
        [HasPermission(Permission.CreateFolder)]
        public async Task<ActionResult<bool>> CreateFolder([FromBody][Required] CreateFolderRequest request)
        {
            //Получаю текущего пользователя
            string? currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (currentUserEmail is null)
            {
                return NotFound("Пользователь не найден");
            }

            bool isSuccessfully = await _folderService.CreateProtocolFolderAsync(request.Name, currentUserEmail, request.AllowedEmployeesIds);

            return Ok(isSuccessfully);
        }

        /// <summary>
        /// Возвращает папку протоколов согласно id
        /// </summary>
        /// <param name="id">Идентификатор папки</param>
        /// <remarks>
        /// пример: 3
        /// </remarks>
        [HttpGet]
        [Route(nameof(GetFolder))]
        [HasPermission(Permission.ReadFolder)]
        public async Task<ActionResult<ProtocolFolder>> GetFolder([Required][Range(1, long.MaxValue, ErrorMessage = "Некорректный Id")] long id)
        {
            ProtocolFolder folder = await _folderService.GetProtocolFolderAsync(id);

            return Ok(folder);
        }

        /// <summary>
        /// Изменяет (переименовывает) папку протоколов
        /// </summary>
        [HttpPost]
        [Route(nameof(UpdateFolder))]
        [HasPermission(Permission.UpdateFolder)]
        public async Task<ActionResult<bool>> UpdateFolder([FromBody][Required] UpdateFolderRequest request)
        {
            //Получаю текущего пользователя
            string? userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail is null)
            {
                return NotFound("Пользователь не найден");
            }

            bool isSuccessfully = await _folderService.UpdateProtocolFolderAsync(request.Id, request.Name, request.CreatedBy, request.AllowedEmployeesIds, userEmail);

            return Ok(isSuccessfully);
        }

        /// <summary>
        /// Возвращает папки протоколов
        /// </summary>
        /// <param name="filter">фильтры и сортировки </param>
        [HttpPost]
        [Route(nameof(GetFolders))]
        [HasPermission(Permission.ReadFolder)]
        public async Task<ActionResult<IEnumerable<ProtocolFolderResponse>>> GetFolders([FromBody] RequestParams? filter = null)
        {
            filter ??= new RequestParams();
            string userEmail = User.Claims
                .First(c => c.Type is "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            (IReadOnlyCollection<ProtocolFolder> folders, int foldersCount) = await _folderService.GetProtocolFoldersAsync(filter, userEmail);

            ProtocolFolderResponse response = new()
            {
                Page = filter.Page,
                PageCount = filter.Count,
                Count = foldersCount,
                Items = folders.ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Удалить (пометить, как удаленную или архивировать) папку протокола
        /// В зависимостти от роли пользователя сервис отправляет заявку на удаление или автоматически удаляет\архивирует
        /// </summary>
        /// <param name="request">Список Id которые нужно удалить</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Folders/RemoveFolder
        ///     {
        ///        "description": "Причина удаления",
        ///        "removedItems": [12, 13]
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [Route(nameof(RemoveFolder))]
        [HasPermission(Permission.RemoveFolder)]
        public async Task<ActionResult<RemoveFolderResponseDto>> RemoveFolder([FromBody][Required] RemoveRequest<long> request)
        {
            return Ok(await _folderService.RemoveProtocolFolderAsync(request.ItemsToRemove!));
        }

        /// <summary>
        /// Получить список пользователей имеющих доступ к папке
        /// </summary>
        /// <param name="folderId">Идентификатор папки</param>
        [HttpGet]
        [Route(nameof(GetEmployeesAllowedToFolder))]
        [HasPermission(Permission.ReadFolder)]
        public async Task<ActionResult<IReadOnlyCollection<Employee>>> GetEmployeesAllowedToFolder(long folderId)
        {
            IReadOnlyCollection<Employee> allowedEmployees = await _folderService.GetEmployeesAllowedToFolder(folderId);
            return Ok(allowedEmployees);
        }

        /// <summary>
        /// Добавить пользователя в список пользователей имеющих доступ к папке
        /// </summary>
        /// <param name="folderId">Идентификатор папки</param>
        /// <param name="employeeId">Идентификатор пользователя</param>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost]
        [Route(nameof(AddAllowedEmployee))]
        [HasPermission(Permission.UpdateFolder)]
        public async Task<ActionResult> AddAllowedEmployee(long folderId, Guid employeeId)
        {
            //Получаю текущего пользователя
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new ArgumentNullException(nameof(userEmail));
            bool isSuccess = await _folderService.AddAllowedEmployeeAsync(folderId, employeeId, userEmail);

            return Ok(isSuccess);
        }

        /// <summary>
        /// Исключить пользователя из списка пользователей имеющих доступ к папке
        /// </summary>
        /// <param name="folderId">Идентифкатор папки</param>
        /// <param name="employeeId">Идентификатор пользователя</param>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost]
        [Route(nameof(RemoveAllowedEmployee))]
        [HasPermission(Permission.UpdateFolder)]
        public async Task<ActionResult> RemoveAllowedEmployee(long folderId, Guid employeeId)
        {
            //Получаю текущего пользователя
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new ArgumentNullException(nameof(userEmail));
            bool isSuccess = await _folderService.RemoveAllowedEmployeeAsync(folderId, employeeId, userEmail);

            return Ok(isSuccess);
        }
    }
}