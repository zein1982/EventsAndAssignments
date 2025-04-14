using System.ComponentModel.DataAnnotations;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssignmentCountController : ControllerBase
    {
        private readonly IAssignmentCountService _assignmentCountService;

        public AssignmentCountController(IAssignmentCountService assignmentCountService)
        {
            _assignmentCountService = assignmentCountService;
        }

        /// <summary>
        /// Возвращает общее количетсво поручений и количество поручений не в статусе "Готово" для трудозанятого
        /// (не реализовано)
        /// </summary>
        /// <param name="employeeEmail">Email трудозанятого</param>
        [HttpGet]
        [Route(nameof(GetAssignmentsCount))]
        public async Task<ActionResult<AssignmentsCount>> GetAssignmentsCount([Required] string employeeEmail)
        {
            AssignmentsCount result = await _assignmentCountService.GetAssignmentsCount(employeeEmail);

            return Ok(result);
        }
    }
}