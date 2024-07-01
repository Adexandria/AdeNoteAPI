

using System.Security.Claims;

namespace AdeNote.Infrastructure.Utilities.UserConfiguation
{
    /// <summary>
    /// Implemention
    /// </summary>
    public class UserIdentity : IUserIdentity
    {
        /// <summary>
        /// Passes the user id to the property
        /// </summary>
        /// <param name="httpContext">An object used to get the user details</param>
        public UserIdentity(IHttpContextAccessor httpContext)
        {
            Guid.TryParse(httpContext.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "id")?.Value, out Guid id);

            UserId = id;

            Email = httpContext.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Stores the user id of the authenticated user
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Stores the user email of the authenticated user
        /// </summary>
        public string Email { get; set; }
    }
}
