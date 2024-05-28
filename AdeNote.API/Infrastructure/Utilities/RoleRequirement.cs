using AdeNote.Models;
using Microsoft.AspNetCore.Authorization;

namespace AdeNote.Infrastructure.Utilities
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public RoleRequirement(params Role[] roles)
        {
            Roles = roles;  
        }

        public Role[] Roles { get; set; }
    }
}
