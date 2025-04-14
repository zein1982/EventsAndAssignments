using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EventsAndAssignments.API.Authentication
{
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

            return policy
                ?? (AuthorizationPolicy?)new AuthorizationPolicyBuilder()
                    .AddRequirements(new AssignmentRequirement(policyName))
                    //.AddRequirements(new AddCommentRequirement(policyName))
                    .Build();
        }
    }
}