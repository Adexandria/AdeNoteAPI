using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdeNote.Controllers
{
    [Route("api/{bookId}/pages")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PageController : BaseController
    {
        private readonly IPageService _pageService;
        public PageController(IPageService pageService, IUserIdentity userIdentity) : base(userIdentity)
        {
            _pageService = pageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPages(Guid bookId)
        {
            var response = await _pageService.GetAll(bookId);
            return response.Response();
        }

        [HttpGet("{pageId}")]
        public async Task<IActionResult> GetPage(Guid bookId, Guid pageId)
        {
            var response = await _pageService.GetById(bookId, pageId);
            return response.Response();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePage(Guid bookId, PageCreateDTO pageCreate)
        {
            var response = await _pageService.Add(bookId,CurrentUser,pageCreate);
            return response.Response();
        }
        [HttpPost("{pageId}/labels")]
        public async Task<IActionResult> AddLabelsToPage(Guid bookId, Guid pageId,List<string> labels)
        {
            var response = await _pageService.AddLabels(bookId,CurrentUser,pageId,labels);
            return response.Response();
        }
        [HttpPut("{pageId}")]
        public async Task<IActionResult> UpdatePage(Guid bookId, Guid pageId,PageUpdateDTO pageUpdate)
        {
            var response = await _pageService.Update(bookId,CurrentUser,pageId,pageUpdate);
            return response.Response();
        }

        [HttpDelete("{pageId}/labels")]
        public async Task<IActionResult> RemoveAllLabels(Guid bookId,Guid pageId)
        {
            var response = await _pageService.RemoveAllPageLabels(bookId, CurrentUser,pageId);
            return response.Response();
        }

        [HttpDelete("{pageId}/labels/{title}")]
        public async Task<IActionResult> RemoveAllLabels(Guid bookId, Guid pageId, string title)
        {
            var response = await _pageService.RemovePageLabel(bookId, CurrentUser, pageId,title);
            return response.Response();
        }

        [HttpDelete("{pageId}")]
        public async Task<IActionResult> DeletePage(Guid bookId, Guid pageId)
        {
            var response = await _pageService.Remove(bookId,CurrentUser,pageId);
            return response.Response();
        }
    }
}
