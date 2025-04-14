using EventsAndAssignments.Models.DTO.Common;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EventsAndAssignments.API.Authentication.Assignments
{
    public class EmployeeIsInAssignmentHandler : AuthorizationHandler<AssignmentRequirement, AssignmentResponse>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EmployeeIsInAssignmentHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AssignmentRequirement requirement,
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

            //Получаю все роли для поручения, протокола, папки
            bool isSuperAdmin = currentEmployee.RoleId is 1 || currentEmployeeAllPositionsWithRoles.Values.Any(role => role == 1);
            bool isAssignmentAdmin = currentEmployee.Id == resource.CreatedBy
                || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == resource.CreatedBy);
            bool isAuthor = currentEmployee.Id == resource.Author?.Id
                || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == resource.Author?.Id);
            bool isResponsibleLeader = (resource.ResponsibleLeaders.Count > 0 && currentEmployee.Id == resource.ResponsibleLeaders[0].Employee?.Id)
                || (resource.ResponsibleLeaders.Count > 0
                    && currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == resource.ResponsibleLeaders[0].Employee?.Id));
            bool isResponsibleExecutor = (resource.ResponsibleExecutors.Count > 0 && currentEmployee.Id == resource.ResponsibleExecutors[0].Employee?.Id)
                || (resource.ResponsibleExecutors.Count > 0
                    && currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == resource.ResponsibleExecutors[0].Employee?.Id));
            bool isResponsibleInspector = (resource.ResponsibleInspectors.Count > 0 && currentEmployee.Id == resource.ResponsibleInspectors[0].Employee?.Id)
                || (resource.ResponsibleInspectors.Count > 0
                    && currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == resource.ResponsibleInspectors[0].Employee?.Id));
            bool isProtocolAdmin = currentEmployee.Id == resource.ProtocolCreatedBy
                || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == resource.ProtocolCreatedBy);
            bool isFolderAdmin = currentEmployee.Id == resource.FolderCreatedBy
                || currentEmployeeAllPositionsWithRoles.Keys.Any(id => id == resource.FolderCreatedBy);
            bool isSecondAdmin = resource.AllowedEmployeesNavigation?.Any(user => user.Id == currentEmployee.Id
                || currentEmployeeAllPositionsWithRoles.Keys.Any(guid => guid == user.Id))
                ?? false;

            if (isSuperAdmin
                || isAssignmentAdmin
                || isProtocolAdmin
                || isFolderAdmin
                || isSecondAdmin
                || isAuthor
                || isResponsibleLeader
                || isResponsibleExecutor
                || isResponsibleInspector)
            {
                context.Succeed(requirement);
            }

            await Task.CompletedTask;
        }
    }
}