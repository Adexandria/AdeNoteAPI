using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private IUserService userService;

        public DashboardController(IUserService _userService)
        {
           userService = _userService;
        }


        [HttpGet]
        [Authorize("Owner")]
        public IActionResult GetActiveUsers()
        {
            var response = userService.GetStatistics();
            return response.Response();
        }
    }
}
