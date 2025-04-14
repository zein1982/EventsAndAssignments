using Microsoft.AspNetCore.Authorization;

namespace EventsAndAssignments.API.Authentication.Comments
{
    public class AddCommentRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public AddCommentRequirement(string permission)
        {
            Permission = permission;
        }
    }
}