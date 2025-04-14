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
    public class AssignmentsController : ControllerBase
    {
        private readonly ILogger<AssignmentsController> _logger;
        private readonly IAssignmentsService _assignmentService;
        private readonly IAuthorizationService _authService;

        public AssignmentsController(ILogger<AssignmentsController> logger,
            IAssignmentsService folderService,
            IAuthorizationService authService)
        {
            _logger = logger;
            _assignmentService = folderService;
            _authService = authService;
        }

        /// <summary>
        /// Возвращает поручение по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор поручения</param>
        [HttpGet]
        [Route(nameof(GetAssignmentById))]
        public async Task<ActionResult<AssignmentResponse>> GetAssignmentById([Required] long id)
        {
            string? currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            AssignmentResponse? assignment = await _assignmentService.GetAssignmentById(id, currentUserEmail);

            if (assignment is null)
            {
                return NotFound(new ErrorResponse($"Поручение с id {id} не доступно либо удалено"));
            }

            AuthorizationResult authResult = await _authService
                .AuthorizeAsync(User, assignment, nameof(Permission.EmployeeIsInAssignment));

            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }

            return Ok(assignment);
        }

        /// <summary>
        /// Возвращает поручение согласно версии поручения
        /// </summary>
        /// <param name="request">Данные о группе и версии поручения</param>
        [HttpPost]
        [Route(nameof(GetAssignmentByVersion))]
        public async Task<ActionResult<AssignmentResponse>> GetAssignmentByVersion(
            [FromBody] AssignmentVersionRequestDTO request)
        {
            if (request.GroupId < 1 || request.Version < 1)
            {
                return BadRequest();
            }

            AssignmentResponse? assignment = await _assignmentService
                .GetAssignmentByGroupIdAndVersionAsync(request.GroupId, request.Version, request.Subversion);

            return assignment is null ? NotFound(assignment) : Ok(assignment);
        }

        /// <summary>
        /// Возвращает поручения согласно фильтрам и сортировкам.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <remarks>
        /// Примеры запроса для получения поручений
        ///
        ///     POST /api/Assignments/GetAssignments
        ///     {
        ///         "count": 100,
        ///         "roleId": null,
        ///         "positionId": "34cbf4a3-a9e8-ed11-b3d6-0050569a16c1",
        ///         "page": 1,
        ///         "parentId": null,
        ///         "year": 2100,
        ///         "sorts": [{
        ///             "name": "Created",
        ///             "selected": true,
        ///             "sortdirection": "ascending"
        ///         }],
        ///         "filters": [{
        ///             "name": "Created",
        ///             "filterType": 3,
        ///             "label": "Дата создания",
        ///             "items":[
        ///             {
        ///                 "value": "2023-07-31T07:05:23.000Z",
        ///                 "selected": true
        ///             },
        ///             {
        ///                 "value": "2023-08-01T07:05:23.000Z",
        ///                 "selected": true
        ///             }
        ///         ]
        ///         }]
        ///     }
        ///
        /// </remarks>>
        [HttpPost]
        [Route(nameof(GetAssignments))]
        public async Task<ActionResult<ListResponseDTO<AssignmentResponseShort>>> GetAssignments(
                [FromBody] RequestParams? filter = null)
        {
            filter ??= new RequestParams();
            string currentUserEmail = "Rinat.Salimianov@evraz.com";//User.FindFirst(ClaimTypes.Email)?.Value
                                                                   //?? throw new ArgumentNullException(nameof(currentUserEmail));

            //Получение отфильтрованных записей
            (List<AssignmentResponseShort> items, int count) filtered = await _assignmentService.GetFilteredAssignments(filter, currentUserEmail);
            ListResponseDTO<AssignmentResponseShort> response = new()
            {
                Count = filtered.count,
                Page = filter.Page,
                PageCount = 0,
                Items = filtered.items.ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Возвращает ID поручений согласно фильтрам и сортировкам.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <remarks>
        /// </remarks>>
        [HttpPost]
        [Route(nameof(GetAssignmentsIds))]
        public async Task<ActionResult<List<long>>> GetAssignmentsIds(
                [FromBody] RequestParams? filter = null)
        {
            filter ??= new RequestParams();
            string currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value
                                    ?? throw new ArgumentNullException(nameof(currentUserEmail));

            //Получение отфильтрованных записей
            List<long> filtered = await _assignmentService.GetFilteredAssignmentsIds(filter, currentUserEmail);

            List<long> response = filtered.ToList();

            return Ok(response);
        }

        /// <summary>
        /// Получить список всех версий и подверсий поручения (для построения дерева версий)
        /// </summary>
        /// <param name="assignmentId">Идентификатор группы поручения</param>
        [HttpPost]
        [Route(nameof(GetAllAssignmentVersions))]
        public async Task<ActionResult<ICollection<AssignmentVersionResponse>>> GetAllAssignmentVersions([Required] long assignmentId)
        {
            if (assignmentId < 1)
            {
                return BadRequest();
            }

            ICollection<AssignmentVersionResponse> response =
                await _assignmentService.GetAllAssignmentVersions(assignmentId);

            return Ok(response);
        }

        /// <summary>
        /// Создает новое поручение
        /// </summary>
        [HttpPost]
        [Route(nameof(CreateAssignment))]
        [HasPermission(Permission.CreateAssignment)]
        public async Task<ActionResult<AssignmentResponseShort>> CreateAssignment([Required] long protocolId)
        {
            //Получаю текущего пользователя
            string currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new ArgumentNullException(nameof(currentUserEmail));

            AssignmentResponseShort created = await _assignmentService.CreateAssignmentAsync(protocolId, currentUserEmail);

            return Ok(created);
        }

        /// <summary>
        /// Копирует выбранные поручения
        /// </summary>
        /// <param name="copyAssignment">Список поручений для копирования</param>
        [HttpPost]
        [Route(nameof(CopyAssignment))]
        public async Task<ActionResult<ICollection<AssignmentResponseShort>>> CopyAssignment(
            [FromBody] CopyAssignmentDtoRequest copyAssignment)
        {
            if (copyAssignment.AssignmentsIds is null || copyAssignment.AssignmentsIds.Count is 0)
            {
                return BadRequest("Количество поручений для копирования должно быть больше 0");
            }

            //Получаю текущий email пользователя
            string currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new ArgumentNullException(nameof(currentUserEmail));

            ICollection<AssignmentResponseShort> copied = await _assignmentService
                    .CopyAssignmentsAsync(copyAssignment.AssignmentsIds, copyAssignment.ProtocolId, currentUserEmail);

            return Ok(copied);
        }

        /// <summary>
        /// Обновляет поручение (для поручений в списке у протокола). Короткая версия
        /// </summary>
        /// <param name="assignmentDto">Данные поручения со строки таблицы</param>
        [HttpPost]
        [Route(nameof(UpdateAssignmentShort))]
        public async Task<ActionResult<AssignmentResponseShort>> UpdateAssignmentShort(
            [Required][FromBody] AssignmentShortRequestDto assignmentDto)
        {
            //Получаю текущего пользователя
            string currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new ArgumentNullException(nameof(currentUserEmail));

            //Обновляю информацию о поручении
            AssignmentResponseShort response = await _assignmentService
                .UpdateAssignmentAsync(assignmentDto, currentUserEmail, null);

            return Ok(response);
        }

        /// <summary>
        /// Обновляет и размножает (в случае необходимости) поручение. Полная версия (для данных с основной формы)
        /// </summary>
        /// <param name="assignmentDto">Данные поручения с формы</param>
        [HttpPost]
        [Route(nameof(UpdateAssignment))]
        public async Task<ActionResult<AssignmentResponse>> UpdateAssignment(
            [Required][FromBody] AssignmentRequestDto assignmentDto)
        {
            //Получаю пользователя
            string currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new ArgumentNullException(nameof(currentUserEmail));

            //Обновляю поручения
            AssignmentResponse response = await _assignmentService
                .UpdateAssignmentAsync(assignmentDto, currentUserEmail, assignmentDto.NeedToReturnForRevision);

            return Ok(response);
        }

        /// <summary>
        /// Удаляет поручения
        /// </summary>
        /// <param name="removeRequest">Список идентификаторов поручений подлежащих удалению</param>
        [HttpPost]
        [Route(nameof(RemoveAssignment))]
        [HasPermission(Permission.RemoveAssignment)]
        public async Task<ActionResult> RemoveAssignment([Required][FromBody] RemoveRequest<long> removeRequest)
        {
            if (removeRequest.ItemsToRemove is null || removeRequest.ItemsToRemove.Count is 0)
            {
                return BadRequest("Количество элементов для удаления должно быть больше 0");
            }

            await _assignmentService.RemoveAssignmentsAsync(removeRequest.ItemsToRemove);

            return Ok($"Поручения с идентификаторами: {string.Join(", ", removeRequest.ItemsToRemove)} успешно удалены!");
        }

        /// <summary>
        /// Обновить уведомления по поручению
        /// </summary>
        /// <param name="assignmentsIds">Список идентификаторов поручений</param>
        [HttpPost]
        [Route(nameof(RestoreNotificationsOnAssignments))]
        public async Task<ActionResult<string>> RestoreNotificationsOnAssignments([Required][FromBody] ICollection<long> assignmentsIds)
        {
            //Получаю пользователя
            string currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new ArgumentNullException(nameof(currentUserEmail));

            //Отправляю запрос на восстановление
            string result = await _assignmentService.RestoreNotificationsOnAssignments(assignmentsIds, currentUserEmail);

            return Ok(result);
        }
    }
}