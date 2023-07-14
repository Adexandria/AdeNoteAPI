using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    [Route("api/{bookId}/pages")]
    [ApiController]
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

        [HttpPut("{pageId}")]
        public async Task<IActionResult> UpdatePage(Guid bookId, Guid pageId,PageUpdateDTO pageUpdate)
        {
            var response = await _pageService.Update(bookId,CurrentUser,pageId,pageUpdate);
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
