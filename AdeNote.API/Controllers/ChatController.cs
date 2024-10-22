using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services.ChatService;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    [Route("api/v{version:apiVersion}/chats")]
    [ApiVersion("1.0")]
    [Authorize(Policy = "User")]
    [ApiController]
    public class ChatController : BaseController
    {
        public ChatController(IChatService chatService, IUserIdentity userIdentity) : base(userIdentity)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Sends tweet
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             POST /chats
        ///             
        /// </remarks> 
        /// <param name="newThread">Creates a tweet</param>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost()]
        public async Task<IActionResult> SendTweet(CreateThreadDto newThread)
        {
            var response = await _chatService.CreateThread(newThread, CurrentUser.ToString());

            return response.Response();
        }


        /// <summary>
        ///  Fetches tweet by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             GET /chats/2b170ff4-0615-4794-b9bb-8d2af76398e7
        ///             
        /// </remarks>
        /// <param name="threadId">Thread id</param>
        /// 
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<TweetThreadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet("{threadId}")]
        public async Task<IActionResult> GetTweetsbyId(string threadId)
        {
            var response = await _chatService.GetThread(threadId);

            return response.Response();
        }

        /// <summary>
        /// Fetches all tweets
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             GET /chats
        ///             
        /// </remarks>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<List<TweetThreadDtos>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public async Task<IActionResult> GetTweets()
        {
            var response = await _chatService.GetThreads();

            return response.Response();
        }

        /// <summary>
        /// Update existing tweets
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             PUT /chats
        ///             
        /// </remarks> 
        /// <param name="threadId">Thread id of the existing thread</param>
        /// <param name="updateThread">Thread to update</param>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPut("{threadId}")]
        public async Task<IActionResult> UpdateTweets(string threadId, UpdateThreadDto updateThread)
        {
            var response = await _chatService.UpdateThread(threadId, updateThread);

            return response.Response();
        }


        /// <summary>
        /// Deletes parent and all subthreads
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             DELETE /chats/2b170ff4-0615-4794-b9bb-8d2af76398e7
        ///             
        /// </remarks> 
        /// <param name="threadId">Thread id of the existing thread</param>

        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpDelete("{threadId}")]
        public async Task<IActionResult> DeleteTweets(string threadId)
        {
            var response = await _chatService.DeleteThread(threadId);

            return response.Response();
        }

        private readonly IChatService _chatService;
    }
}
