using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EventsAndAssignments.API.Authentication;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentService _commentService;
        private readonly IAssignmentsService _assignmentService;
        private readonly IAuthorizationService _authService;

        public CommentController(
            ILogger<CommentController> logger,
            ICommentService commentService,
            IAssignmentsService assignmentsService,
            IAuthorizationService authorizationService)
        {
            _logger = logger;
            _commentService = commentService;
            _authService = authorizationService;
            _assignmentService = assignmentsService;
        }

        [HttpPost]
        [Route(nameof(RemoveComment))]
        public async Task<ActionResult<long>> RemoveComment(
            [Required][Range(1, long.MaxValue)] long comentId)
        {
            long removedId = await _commentService.RemoveCommentAsync(comentId);
            return Ok(removedId);
        }

        /// <summary>
        /// Получить список комментариев для конкретного поручения
        /// </summary>
        /// <param name="assignmentId">Идентификатор поручения</param>
        /// <returns>Список комментариев</returns>
        [HttpGet]
        [Route(nameof(GetComments))]
        public async Task<ActionResult<IReadOnlyCollection<CommentResponseDto>>> GetComments([Required] long assignmentId)
        {
            string currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new ArgumentNullException(nameof(currentUserEmail));
            ICollection<CommentResponseDto> response = await _commentService.GetAllCommentsForAssignmentAsync(assignmentId, currentUserEmail);

            return Ok(response);
        }

        /// <summary>
        /// Получить последний комментарий по идентификатору поручения
        /// </summary>
        /// <returns>Последний комментарий по выбранному поручению</returns>
        [HttpGet]
        [Route(nameof(GetLastComment))]
        public async Task<ActionResult<CommentResponseDto>> GetLastComment([Required] long assignmentId)
        {
            CommentResponseDto? response = await _commentService.GetLastAsync(assignmentId);

            if (response is null)
            {
                return NoContent();
            }

            return Ok(response);
        }

        /// <summary>
        /// Создает комментарий для текущего поручения
        /// </summary>
        /// <param name="comment">Данные комментария</param>
        [HttpPost]
        [Route(nameof(CreateComment))]
        public async Task<ActionResult<CommentResponseDto>> CreateComment([Required] CommentRequestDto comment)
        {
            //Получаю текущего пользователя
            string currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new ArgumentNullException(nameof(currentUserEmail));

            AssignmentResponse? assignment = await _assignmentService.GetAssignmentById(comment.AssignmentId, currentUserEmail);

            if (assignment is null)
            {
                return NotFound(new ErrorResponse($"Поручение с id {comment.AssignmentId} не найдено"));
            }

            AuthorizationResult authResult = await _authService
                .AuthorizeAsync(User, assignment, nameof(Permission.CreateComment));

            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }

            CommentResponseDto response = await _commentService
                .CreateAsync(
                    comment.Content,
                    comment.AssignmentId,
                    currentUserEmail,
                    comment.StatusCard);

            return Ok(response);
        }
    }
}