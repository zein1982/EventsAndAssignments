using EventsAndAssignments.Models.DTO.Common;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EventsAndAssignments.API.Authentication
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<AssignmentRequirement>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AssignmentRequirement requirement)
        {
            System.Security.Claims.ClaimsPrincipal employee = context.User;

            string email = context.User.Identities.First().Claims.Last().Value;

            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            IPermissionService permissionService = scope.ServiceProvider
                .GetRequiredService<IPermissionService>();

            IEmployeeService employeeService = scope.ServiceProvider
                .GetRequiredService<IEmployeeService>();
            Employee? emp = employeeService.GetEmployeeByEmail(email);

            List<string> permissions = await permissionService
                .GetPermissionsAsync(emp!.Id);

            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}