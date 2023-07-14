using System.Security.Claims;

namespace AdeNote.Infrastructure.Utilities
{
    public class UserIdentity : IUserIdentity
    {
        public UserIdentity(IHttpContextAccessor httpContext)
        {
            Guid.TryParse(httpContext.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value, out Guid id);

            UserId = id;
        }
        public Guid UserId { get; set; }
    }
}
