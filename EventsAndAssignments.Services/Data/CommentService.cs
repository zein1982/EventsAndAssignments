using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Exceptions;
using EventsAndAssignments.Services.Interfaces;
using MapsterMapper;

namespace EventsAndAssignments.Services.Data
{
    public class CommentService : ICommentService
    {
        private readonly ICommentGateway _commentGateway;
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;

        public CommentService(
            ICommentGateway commentGateway,
            IMapper mapper,
            IEmployeeService employeeService)
        {
            _commentGateway = commentGateway;
            _mapper = mapper;
            _employeeService = employeeService;
        }

        public async Task<CommentResponseDto> CreateAsync(
            string? content,
            long? assignmentId,
            string currentUserEmail,
            long? assignmentStatus)
        {
            //Получаю текущего пользователя
            Models.DTO.Common.Employee? currentEmployee = _employeeService.GetEmployeeByEmail(currentUserEmail) ??
                throw new EntityNotFoundException();

            Comment newComment = new()
            {
                Content = content,
                AssignmentId = assignmentId,
                CreatedBy = currentEmployee.Id,
                UpdatedBy = currentEmployee.Id,
                StatusCreated = assignmentStatus
            };

            Comment created = await _commentGateway.CreateAsync(newComment);
            CommentResponseDto response = _mapper.Map<CommentResponseDto>(created);

            return response;
        }

        public async Task<CommentResponseDto?> GetLastAsync(long assignmentId)
        {
            Comment? comment = await _commentGateway.GetLastAsync(assignmentId);
            CommentResponseDto? response = _mapper.Map<CommentResponseDto>(comment!);

            return response;
        }

        public async Task<ICollection<CommentResponseDto>> GetAllCommentsForAssignmentAsync(
            long assignmentId,
            string employeeMail)
        {
            //получаем текущего пользователя
            Models.DTO.Common.Employee? currntEmployee = _employeeService.GetEmployeeByEmail(employeeMail);

            IReadOnlyCollection<Comment> comments = await _commentGateway.GetAllCommentsForAssignmentAsync(assignmentId);
            ICollection<CommentResponseDto> response = _mapper.Map<ICollection<CommentResponseDto>>(comments);

            //устанавливаем свойство на 
            foreach (var comment in response)
            {
                comment.UserCanRemoveComment = currntEmployee.RoleId is 1
                    || (currntEmployee.RoleId is 2 && comment.CreatedBy == currntEmployee.Id)
                    ? comment.UserCanRemoveComment = true
                    : comment.UserCanRemoveComment = false;
            }

            return response;
        }

        public async Task<long> RemoveCommentAsync(
            long commentId)
        {
            long removedId = await _commentGateway.RemoveCommentAsync(commentId);

            return removedId;
        }

        public async Task UpdateCommentAsync(long commentId, string newText) =>
            await _commentGateway.UpdateComment(commentId, newText);
    }
}