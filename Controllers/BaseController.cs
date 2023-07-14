using AdeNote.Infrastructure.Utilities;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using TasksLibrary.Architecture.Application;

namespace AdeNote.Controllers
{
    public class BaseController : ControllerBase
    {
        public BaseController(IContainer container, ITaskApplication application)
        {
            Container = container;
            Application = application;
        }

        public BaseController(IContainer container,ITaskApplication application, IUserIdentity userIdentity)
        {
            Container = container;
            Application = application;
            CurrentUser = userIdentity.UserId;
        }
        protected IContainer Container { get; set; }
        protected ITaskApplication Application { get; set; }
        protected Guid CurrentUser { get; set;}
    }
}
