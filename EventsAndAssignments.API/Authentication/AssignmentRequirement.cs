using Microsoft.AspNetCore.Authorization;

namespace EventsAndAssignments.API.Authentication
{
    public class AssignmentRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public AssignmentRequirement(string permission)
        {
            Permission = permission;
        }
    }
}