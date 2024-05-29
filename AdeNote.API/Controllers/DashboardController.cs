using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private IUserService userService;

        public DashboardController(IUserService _userService, IUserIdentity userIdentity) : base(userIdentity) 
        {
           userService = _userService;
        }


        [HttpGet("admin")]
        [Authorize("Owner")]
        public IActionResult GetActiveUsers()
        {
            var response = userService.GetStatistics();
            return response.Response();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var response = await userService.GetUser(CurrentUser);
            return response.Response();
        }
    }
}
