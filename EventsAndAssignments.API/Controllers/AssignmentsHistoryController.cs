using System.ComponentModel.DataAnnotations;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AssignmentsHistoryController : ControllerBase
    {
        private readonly ILogger<AssignmentsHistoryController> _logger;
        private readonly IAssignmentHistoryService _assignmentHistoryService;

        public AssignmentsHistoryController(
            ILogger<AssignmentsHistoryController> logger,
            IAssignmentHistoryService assignmentHistoryService)
        {
            _logger = logger;
            _assignmentHistoryService = assignmentHistoryService;
        }

        [HttpGet]
        [Route(nameof(GetAssignmentHistoryById))]
        public async Task<ActionResult<ICollection<AssignmentHistoryResponseDto>>> GetAssignmentHistoryById(
            [Required] long assignmentId)
        {
            ICollection<AssignmentHistoryResponseDto> result =
                await _assignmentHistoryService.GetAll(assignmentId);

            if (result.Count is 0)
            {
                return NoContent();
            }

            return Ok(result);
        }
    }
}