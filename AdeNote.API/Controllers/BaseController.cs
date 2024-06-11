using AdeNote.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    /// <summary>
    /// A base controller
    /// </summary>
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// A parameterless constructor
        /// </summary>
        public BaseController()
        {

        }


        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="userIdentity">An interface that interacts with the user. This fetches the current user details</param>
        public BaseController(IUserIdentity userIdentity)
        {
            CurrentUser = userIdentity.UserId;
            CurrentEmail = userIdentity.Email;

        }

        /// <summary>
        /// A guid property that hold the value of the current user id
        /// </summary>
        protected Guid CurrentUser { get; set;}

        /// <summary>
        /// Email of the authenticated user
        /// </summary>
        protected string CurrentEmail { get; set; }
    }
}
