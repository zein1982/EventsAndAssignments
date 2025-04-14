using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.Interfaces;

namespace EventsAndAssignments.Services.Data
{
    public class AssignmentCountService : IAssignmentCountService
    {
        readonly IEmployeeService _employeeService;
        private readonly IAssignmentsGateway _assignmentGateway;

        public AssignmentCountService(
            IEmployeeService employeeService,
            IAssignmentsGateway assignmentGateway)
        {
            _employeeService = employeeService;
            _assignmentGateway = assignmentGateway;
        }

        public async Task<AssignmentsCount> GetAssignmentsCount(string employeeEmail)
        {
            Models.DTO.Common.Employee? employee = _employeeService.GetEmployeeByEmail(employeeEmail);

            if (employee is null)
            {
                return new AssignmentsCount { TotalAssignments = 0, UnfinishedAssignments = 0 };
            }

            //Получаю все должности текущего пользователя
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles = await _employeeService
                .GetAllEmployeePositionsByPositionId(employee.Id);

            AssignmentsCount result = new()
            {
                TotalAssignments =
                    await _assignmentGateway.GetEmployeeAssignmentCount(employee.Id, currentEmployeeAllPositionsWithRoles),
                UnfinishedAssignments =
                    await _assignmentGateway.GetEmployeeAssignmentUnFinishedCount(employee.Id, currentEmployeeAllPositionsWithRoles)
            };

            return result;
        }
    }
}