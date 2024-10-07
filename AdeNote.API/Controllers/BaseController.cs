using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
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

        public BaseController(IUserIdentity userIdentity, Application application)
        {
            CurrentUser = userIdentity.UserId;
            CurrentEmail = userIdentity.Email;
            Application = application;
        }

        public BaseController(Application application)
        {
            Application = application;
        }

        /// <summary>
        /// A guid property that hold the value of the current user id
        /// </summary>
        protected Guid CurrentUser { get; set;}

        /// <summary>
        /// Email of the authenticated user
        /// </summary>
        protected string CurrentEmail { get; set; }

        /// <summary>
        /// Manages request handlers
        /// </summary>
        protected Application Application { get; set; }
    }
}
