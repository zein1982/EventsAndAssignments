using EventsAndAssignments.Models.DTO.Common;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EventsAndAssignments.API.Authentication.Comments
{
    public class EmployeeCanAddCommentHandler : AuthorizationHandler<AddCommentRequirement, AssignmentResponse>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EmployeeCanAddCommentHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AddCommentRequirement requirement,
            AssignmentResponse resource)
        {
            System.Security.Claims.ClaimsPrincipal employee = context.User;

            string email = context.User.Identities.First().Claims.Last().Value;

            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            IPermissionService permissionService = scope.ServiceProvider
                .GetRequiredService<IPermissionService>();

            IEmployeeService employeeService = scope.ServiceProvider
                .GetRequiredService<IEmployeeService>();
            Employee? currentEmployee = employeeService.GetEmployeeByEmail(email);

            if (currentEmployee is null)
            {
                await Task.CompletedTask;
                return;
            }

            //Получаю все должности текущего пользователя
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles = await employeeService
                .GetAllEmployeePositionsByPositionId(currentEmployee.Id);

            bool isSuperAdmin = currentEmployee.RoleId is 1 || currentEmployeeAllPositionsWithRoles.Values.Any(role => role == 1);
            bool isAdmin = currentEmployee.RoleId is 2 || currentEmployeeAllPositionsWithRoles.Values.Any(role => role == 2);
            bool isResponsibleLeader = (resource.ResponsibleLeaders.Count > 0 && currentEmployee.Id == resource.ResponsibleLeaders[0].Employee?.Id)
                || (resource.ResponsibleLeaders.Count > 0
                    && currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == resource.ResponsibleLeaders[0].Employee?.Id));
            bool isResponsibleExecutor = (resource.ResponsibleExecutors.Count > 0 && currentEmployee.Id == resource.ResponsibleExecutors[0].Employee?.Id)
                || (resource.ResponsibleExecutors.Count > 0
                    && currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == resource.ResponsibleExecutors[0].Employee?.Id));
            bool isResponsibleInspector = (resource.ResponsibleInspectors.Count > 0 && currentEmployee.Id == resource.ResponsibleInspectors[0].Employee?.Id)
                || (resource.ResponsibleInspectors.Count > 0
                    && currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == resource.ResponsibleInspectors[0].Employee?.Id));

            if (isSuperAdmin
                || isAdmin
                || (ResponsibleLeadersCanAddComment(resource) && isResponsibleLeader)
                || (ResponsibleExecutorCanAddComment(resource) && isResponsibleExecutor)
                || (ResponsibleInspectorCanAddComment(resource) && isResponsibleInspector))
            {
                context.Succeed(requirement);
            }

            await Task.CompletedTask;
        }

        private bool ResponsibleLeadersCanAddComment(AssignmentResponse resource) =>
            resource.Status is 3 or 5 or 6;

        private bool ResponsibleExecutorCanAddComment(AssignmentResponse resource) =>
            resource.Status is 3;

        private bool ResponsibleInspectorCanAddComment(AssignmentResponse resource) =>
            resource.Status is 4;
    }
}