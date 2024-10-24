using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services.ChatService;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    [Route("api/{threadId}/sub-threads")]
    [ApiController]
    public class SubChatsController : BaseController
    {
        public SubChatsController(IChatService chatService, IUserIdentity userIdentity) : base(userIdentity)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Creates sub thread in an existing thread
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             POST /2b170ff4-0615-4794-b9bb-8d2af76398e7/sub-threads
        ///             
        /// </remarks> 
        /// <param name="newThread">Thread to create</param>
        /// <param name="threadId">Thread id</param>
        /// <param name="replyEmails">Users </param>
        [HttpPost()]
        public async Task<IActionResult> SendSubTweet(CreateThreadDto newThread, [FromRoute] string threadId, [FromQuery] string[] replyEmails)
        {
            var response = await _chatService.CreateSubThread(newThread, CurrentEmail, replyEmails, threadId);

            return response.Response();
        }

        /// <summary>
        /// Fetches a subthread from an existing thread
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             GET /2b170ff4-0615-4794-b9bb-8d2af76398e7/2b170ff4-0615-4794-b9bb-8d2af76398e8
        ///             
        /// </remarks> 
        /// <param name="subThreadId">Sub thread id</param>
        /// <param name="threadId">Thread id</param>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<SubThreadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet("{subThreadId}")]
        public async Task<IActionResult> FetchSubTweet(string subThreadId, string threadId)
        {
            var response = await _chatService.GetSubThread(subThreadId, threadId);

            return response.Response();
        }


        /// <summary>
        /// Updates an existing sub thread
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             PUT /2b170ff4-0615-4794-b9bb-8d2af76398e7/
        ///             
        /// </remarks> 
        /// <param name="subThreadId">Sub thread id</param>
        /// <param name="threadId">Thread id</param>
        /// <param name="updateThreadDto">Model to update an existing sub thread</param>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPut("{subThreadId}")]
        public async Task<IActionResult> UpdateSubTweet(string subThreadId, string threadId, UpdateThreadDto updateThreadDto)
        {
            var response = await _chatService.UpdateSubThread(subThreadId,threadId,updateThreadDto);

            return response.Response();
        }

        /// <summary>
        /// Deletes an existing thread
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             DELETE /2b170ff4-0615-4794-b9bb-8d2af76398e7
        ///             
        /// </remarks> 
        /// <param name="subThreadId">Sub thread id</param>
        /// <param name="threadId">Thread id</param>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpDelete("{subThreadId}")]
        public async Task<IActionResult> DeleteSubTweet(string subThreadId, string threadId)
        {
            var response = await _chatService.DeleteSubThread(subThreadId, threadId);

            return response.Response();
        }

        private readonly IChatService _chatService;
    }
}
