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
        /// <param name="userIds">Users </param>
        [HttpPost()]
        public async Task<IActionResult> SendSubTweet(CreateThreadDto newThread, [FromRoute] string threadId, [FromQuery] string[] userIds)
        {
            var response = await _chatService.CreateSubThread(newThread, CurrentUser.ToString(), userIds, threadId);

            return response.Response();
        }

        [HttpGet("{subThreadId}")]
        public async Task<IActionResult> FetchSubTweet(string subThreadId, string threadId)
        {
            var response = await _chatService.GetSubThread(subThreadId, threadId);

            return response.Response();
        }

        [HttpPut("{subThreadId}")]
        public async Task<IActionResult> UpdateSubTweet(string subThreadId, string threadId, UpdateThreadDto updateThreadDto)
        {
            var response = await _chatService.UpdateSubThread(subThreadId,threadId,updateThreadDto);

            return response.Response();
        }

        [HttpDelete("{subThreadId}")]
        public async Task<IActionResult> DeleteSubTweet(string subThreadId, string threadId)
        {
            var response = await _chatService.DeleteSubThread(subThreadId, threadId);

            return response.Response();
        }

        private readonly IChatService _chatService;
    }
}
