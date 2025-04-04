using Microsoft.AspNetCore.Authorization;

namespace Helpers
{
    public class CustomRolePolicyHandler : AuthorizationHandler<CustomRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRoleRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail(new AuthorizationFailureReason((IAuthorizationHandler)this, "Authentification requise"));
                return Task.CompletedTask;
            }

            if (!context.User.IsInRole(requirement.RequiredRole))
            {
                context.Fail(new AuthorizationFailureReason((IAuthorizationHandler)this, "Accès refusé. Vous n'avez pas le rôle requis."));
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
    public class CustomRoleRequirement : IAuthorizationRequirement
    {
        public string RequiredRole { get; }
        public CustomRoleRequirement(string role) => RequiredRole = role;
    }
}
