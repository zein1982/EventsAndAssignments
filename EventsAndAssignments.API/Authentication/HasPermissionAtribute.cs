using Microsoft.AspNetCore.Authorization;

namespace EventsAndAssignments.API.Authentication
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permission permission)
            : base(policy: permission.ToString())
        {
        }
    }
}