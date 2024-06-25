using AdeNote.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AdeNote.Infrastructure.Utilities.AuthorisationHandler
{
    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            var user = context.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var userRole = context.User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userRole))
            {
                context.Fail();
                return;
            }

            var isRole = Enum.TryParse(userRole, out Role role);

            if (!isRole)
            {
                context.Fail();
                return;
            }

            var isAuthorised = requirement.Roles.Contains(role);

            if (!isAuthorised)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }
    }
}
