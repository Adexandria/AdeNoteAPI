using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services.Statistics;
using AdeNote.Infrastructure.Services.UserSettings;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    [Route("api/v{version:apiVersion}/dashboard")]
    [ApiVersion("1.0")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private IUserService userService;
        private readonly IStatisticsService statisticsService;

        public DashboardController(IUserService _userService,
            IStatisticsService _statisticsService,
            IUserIdentity userIdentity) : base(userIdentity) 
        {
           userService = _userService;
            statisticsService = _statisticsService;
        }


        [HttpGet("admin")]
        [Authorize("Owner")]
        public IActionResult GetStatistics()
        {
            var response = statisticsService.GetStatistics();
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
