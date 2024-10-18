using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services.ChatService;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Asp.Versioning;
using ChattyPie.Application;
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

        [HttpPost("send")]
        public async Task<IActionResult> SendTweet(CreateThreadDto newThread)
        {
            var response = await _chatService.CreateThread(newThread, CurrentUser.ToString());

            return response.Response();
        }

        [HttpPost("send/{threadId}")]
        public async Task<IActionResult> SendSubTweet(CreateThreadDto newThread, [FromRoute]string threadId, [FromQuery]string[] userIds)
        {
            var response = await _chatService.CreateSubThread(newThread,CurrentUser.ToString(),userIds,threadId);

            return response.Response();
        }

        [HttpGet]
        public async Task<IActionResult> GetTweets(string threadId)
        {
            var thread = await _chatService.GetThread(threadId);

            return Ok(thread);
        }

        private IChatService _chatService;
    }
}
