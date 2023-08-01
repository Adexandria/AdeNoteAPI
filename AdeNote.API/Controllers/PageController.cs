using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AdeNote.Controllers
{
    /// <summary>
    /// Handles the page management for a particular book.
    /// 
    /// Supports version 1
    /// </summary>
    [Route("api/v{version:apiVersion}/{bookId}/pages")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PageController : BaseController
    {
        private readonly IPageService _pageService;

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="pageService">An interface that interacts with the page table</param>
        /// <param name="userIdentity">An interface that interacts with the user. This fetches the current user details</param>
        public PageController(IPageService pageService, IUserIdentity userIdentity) : base(userIdentity)
        {
            _pageService = pageService;
        }


        /// <summary>
        /// Fetches all pages in a book
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             GET /20b1204e-fad5-4a90-a78e-bc3b988afd60/pages
        /// </remarks>
        /// <param name="bookId">A book id</param>
        /// <returns>A list of pages</returns>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<IEnumerable<PageDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public async Task<IActionResult> GetAllPages(Guid bookId)
        {
            var response = await _pageService.GetAll(bookId);
            return response.Response();
        }

        /// <summary>
        /// fetches a single page in a book
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///         
        ///             GET /20b1204e-fad5-4a90-a78e-bc3b988afd60/pages/20b1204e-fad5-4a90-a78e-bc3b988afd60
        /// </remarks>
        /// <param name="bookId">A book id</param>
        /// <param name="pageId">A page id</param>
        /// <returns> A single page</returns>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="400"> Returns if experiencing client issues</response>
        ///  <response code ="404"> Returns if not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<PageDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet("{pageId}")]
        public async Task<IActionResult> GetPage(Guid bookId, Guid pageId)
        {
            var response = await _pageService.GetById(bookId, pageId);
            return response.Response();
        }


        /// <summary>
        /// Creates a single page in a book
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///         
        ///                 POST /20b1204e-fad5-4a90-a78e-bc3b988afd60/pages
        ///                 {
        ///                     "title": "string",
        ///                     "content": "string"
        ///                 }
        /// </remarks>
        /// <param name="bookId">A book id</param>
        /// <param name="pageCreate">A page id</param>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="400"> Returns if experiencing client issues</response>
        ///  <response code ="500"> Returns if experiencing server issues</response>
        ///  <response code ="404"> Returns if not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public async Task<IActionResult> CreatePage(Guid bookId, PageCreateDTO pageCreate)
        {
            var response = await _pageService.Add(bookId,CurrentUser,pageCreate);
            return response.Response();
        }

        /// <summary>
        /// Adds labels to a page
        /// </summary>
        /// <remarks>
        /// Sample requests:
        /// 
        ///             POST /20b1204e-fad5-4a90-a78e-bc3b988afd60/pages/20b1204e-fad5-4a90-a78e-bc3b988afd60/labels
        ///             {
        ///              [
        ///                 "Ef Core"
        ///              ]
        ///             }
        /// </remarks>
        /// <param name="bookId">A book id</param>
        /// <param name="pageId">A page id</param>
        /// <param name="labels">A list of labels</param>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="400"> Returns if experiencing client issues</response>
        ///  <response code ="500"> Returns if experiencing server issues</response>
        ///  <response code ="404"> Returns if not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("{pageId}/labels")]
        public async Task<IActionResult> AddLabelsToPage(Guid bookId, Guid pageId,List<string> labels)
        {
            var response = await _pageService.AddLabels(bookId,CurrentUser,pageId,labels);
            return response.Response();
        }


        /// <summary>
        /// Updates an existing page
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///         
        ///                 PUT /20b1204e-fad5-4a90-a78e-bc3b988afd60/pages/20b1204e-fad5-4a90-a78e-bc3b988afd60
        ///                 {
        ///                     "title": "string",
        ///                     "content": "string"
        ///                 }
        /// </remarks>
        /// <param name="bookId">A book id</param>
        /// <param name="pageId">A page id</param>
        /// <param name="pageUpdate">A model to update an existing page</param>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="400"> Returns if experiencing client issues</response>
        ///  <response code ="500"> Returns if experiencing server issues</response>
        ///  <response code ="404"> Returns if not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPut("{pageId}")]
        public async Task<IActionResult> UpdatePage(Guid bookId, Guid pageId,PageUpdateDTO pageUpdate)
        {
            var response = await _pageService.Update(bookId,CurrentUser,pageId,pageUpdate);
            return response.Response();
        }

        /// <summary>
        /// Removes all label from a page
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///         
        ///                 DELETE /20b1204e-fad5-4a90-a78e-bc3b988afd60/pages/20b1204e-fad5-4a90-a78e-bc3b988afd60/labels
        /// </remarks>
        /// <param name="bookId">A book id</param>
        /// <param name="pageId">A page id</param>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="400"> Returns if experiencing client issues</response>
        ///  <response code ="500"> Returns if experiencing server issues</response>
        ///  <response code ="404"> Returns if not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpDelete("{pageId}/labels")]
        public async Task<IActionResult> RemoveAllLabels(Guid bookId,Guid pageId)
        {
            var response = await _pageService.RemoveAllPageLabels(bookId, CurrentUser,pageId);
            return response.Response();
        }

        /// <summary>
        /// Removes a single label from a page
        /// </summary>
        /// <remarks>
        ///  Sample request:
        ///         
        ///                 DELETE /20b1204e-fad5-4a90-a78e-bc3b988afd60/pages/20b1204e-fad5-4a90-a78e-bc3b988afd60/labels/search?title=Ef core
        /// </remarks>
        /// <param name="bookId">A book id</param>
        /// <param name="pageId">A page id</param>
        /// <param name="title">A label</param>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="400"> Returns if experiencing client issues</response>
        ///  <response code ="500"> Returns if experiencing server issues</response>
        ///  <response code ="404"> Returns if not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpDelete("{pageId}/labels/search")]
        public async Task<IActionResult> RemoveAllLabels(Guid bookId, Guid pageId, string title)
        {
            var response = await _pageService.RemovePageLabel(bookId, CurrentUser, pageId,title);
            return response.Response();
        }


        /// <summary>
        /// Removes a page froma book
        /// </summary>
        /// <remarks>
        ///  Sample request:
        ///         
        ///                 DELETE /20b1204e-fad5-4a90-a78e-bc3b988afd60/pages/20b1204e-fad5-4a90-a78e-bc3b988afd60
        /// </remarks>
        /// <param name="bookId">A book id</param>
        /// <param name="pageId">A page id</param>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="400"> Returns if experiencing client issues</response>
        ///  <response code ="500"> Returns if experiencing server issues</response>
        ///  <response code ="404"> Returns if not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpDelete("{pageId}")]
        public async Task<IActionResult> DeletePage(Guid bookId, Guid pageId)
        {
            var response = await _pageService.Remove(bookId,CurrentUser,pageId);
            return response.Response();
        }
    }
}
