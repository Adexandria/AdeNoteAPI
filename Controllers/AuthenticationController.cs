using AdeNote.Infrastructure.Extension;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using TasksLibrary.Application.Commands.CreateUser;
using TasksLibrary.Application.Commands.Login;
using TasksLibrary.Architecture.Application;

namespace AdeNote.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        public AuthenticationController(IContainer container, ITaskApplication application) : base(container, application)
        {
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(CreateUserCommand command)
        {
            var response = await Application.ExecuteCommand<CreateUserCommand, CreateUserDTO>(Container, command);
            return response.Response();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var response = await Application.ExecuteCommand<LoginCommand, LoginDTO>(Container, command);
            return response.Response();
        }
    }
}
