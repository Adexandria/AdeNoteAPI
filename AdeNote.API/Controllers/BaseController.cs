using AdeNote.Infrastructure.Utilities;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using TasksLibrary.Architecture.Application;

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
        }

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="container">A container that contains all the built dependencies</param>
        /// <param name="application">An interface used to interact with the layers</param>
        public BaseController(IContainer container, ITaskApplication application)
        {
            Container = container;
            Application = application;
        }

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="container">A container that contains all the built dependencies</param>
        /// <param name="application">An interface used to interact with the layers</param>
        /// <param name="userIdentity">An interface that interacts with the user. This fetches the current user details</param>
        public BaseController(IContainer container,ITaskApplication application, IUserIdentity userIdentity)
        {
            Container = container;
            Application = application;
            CurrentUser = userIdentity.UserId;
        }
        /// <summary>
        /// A container property that contains all the registered dependencies
        /// </summary>
        protected IContainer Container { get; set; }

        /// <summary>
        /// An interface that is used to interact with different layers
        /// </summary>
        protected ITaskApplication Application { get; set; }

        /// <summary>
        /// A guid property that hold the value of the current user id
        /// </summary>
        protected Guid CurrentUser { get; set;}
    }
}
