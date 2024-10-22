using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services.Statistics;
using AdeNote.Infrastructure.Services.UserSettings;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models.DTOs;
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

        /// <summary>
        /// Get dashboard for admin
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             GET /dashboard/admin
        ///             
        /// </remarks> 
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<StatisticsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet("admin")]
        [Authorize("Owner")]
        public IActionResult GetStatistics()
        {
            var response = statisticsService.GetStatistics();
            return response.Response();
        }


        /// <summary>
        ///  Fetches dashboard for users
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             GET /dashboard
        ///             
        /// </remarks> 
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<UsersDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var response = await userService.GetUser(CurrentUser);
            return response.Response();
        }
    }
}
