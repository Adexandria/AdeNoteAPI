using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Requests.CreatePage;
using AdeNote.Infrastructure.Requests.CreatePageLabels;
using AdeNote.Infrastructure.Requests.GetAllPages;
using AdeNote.Infrastructure.Requests.GetPagesById;
using AdeNote.Infrastructure.Requests.RemoveAllPageLabels;
using AdeNote.Infrastructure.Requests.RemovePage;
using AdeNote.Infrastructure.Requests.RemovePageLabel;
using AdeNote.Infrastructure.Requests.TranslatePage;
using AdeNote.Infrastructure.Requests.UpdatePage;
using AdeNote.Infrastructure.Services.PageSettings;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Infrastructure.Utilities.ValidationAttributes;
using AdeNote.Models.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


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
    [Authorize(Policy = "User")]
    public class PageController : BaseController
    {
        private readonly IPageService _pageService;

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="pageService">An interface that interacts with the page table</param>
        /// <param name="userIdentity">An interface that interacts with the user. This fetches the current user details</param>
        public PageController(IPageService pageService, IUserIdentity userIdentity, Application application) : base(userIdentity,application)
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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<IEnumerable<PageDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public async Task<IActionResult> GetAllPages([ValidGuid("Invalid book id")]Guid bookId)
        {
            var response = await Application.SendAsync(new GetAllPagesRequest()
            {
                BookId = bookId
            });
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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<PageDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet("{pageId}")]
        public async Task<IActionResult> GetPage([ValidGuid("Invalid book id")] Guid bookId, [ValidGuid("Invalid page id")] Guid pageId)
        {
            var response = await Application.SendAsync<GetPageByIdRequest, PageDTO>(new GetPageByIdRequest()
            {
                BookId = bookId, 
                PageId = pageId
            });

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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public async Task<IActionResult> CreatePage([ValidGuid("Invalid book id")] Guid bookId, PageCreateDTO pageCreate)
        {
            var response = await Application.SendAsync(new CreatePageRequest()
            {
                BookId = bookId,
                Content = pageCreate.Content,
                Title = pageCreate.Title,
                UserId = CurrentUser
            });
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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("{pageId}/labels")]
        public async Task<IActionResult> AddLabelsToPage([ValidGuid("Invalid book id")] Guid bookId, [ValidGuid("Invalid page id")] Guid pageId, 
            [ValidCollection("Invalid labels")]List<string> labels)
        {
            var response = await Application.SendAsync(new CreatePageLabelsRequest()
            {
                BookId = bookId,
                PageId = pageId,
                Labels = labels,
                UserId= CurrentUser
            });
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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPut("{pageId}")]
        public async Task<IActionResult> UpdatePage([ValidGuid("Invalid book id")] Guid bookId, [ValidGuid("Invalid page id")] Guid pageId,PageUpdateDTO pageUpdate)
        {
            var response = await Application.SendAsync(new UpdatePageRequest() 
            { 
                UserId = CurrentUser,
                BookId = bookId,
                PageId = pageId,
                UpdatePage = pageUpdate
            });

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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpDelete("{pageId}/labels")]
        public async Task<IActionResult> RemoveAllLabels([ValidGuid("Invalid book id")] Guid bookId, [ValidGuid("Invalid page id")] Guid pageId)
        {
            var response = await Application.SendAsync(new RemoveAllPageLabelsRequest() 
            { 
                PageId= pageId,
                BookId= bookId,
                UserId = CurrentUser
            });

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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpDelete("{pageId}/labels/search")]
        public async Task<IActionResult> RemoveAllLabels([ValidGuid("Invalid book id")] Guid bookId, [ValidGuid("Invalid page id")] Guid pageId, string title)
        {
            var response = await Application.SendAsync(new RemovePageLabelRequest() 
            {
                UserId = CurrentUser,
                PageId = pageId,
                BookId = bookId,
                Title = title
            });

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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpDelete("{pageId}")]
        public async Task<IActionResult> DeletePage([ValidGuid("Invalid book id")] Guid bookId, [ValidGuid("Invalid page id")] Guid pageId)
        {
            Infrastructure.Utilities.ActionResult response = await Application.SendAsync(new RemovePageRequest()
            {
                PageId = pageId,
                BookId=bookId,
                UserId = CurrentUser
            });
            return response.Response();
        }



        /// <summary>
        /// Translates page to another language
        /// </summary>
        /// <remarks>
        ///  Sample request:
        ///         
        ///                 POST /20b1204e-fad5-4a90-a78e-bc3b988afd60/pages/20b1204e-fad5-4a90-a78e-bc3b988afd60/translate?to=de
        /// </remarks>
        /// <param name="bookId">A book id</param>
        /// <param name="pageId">A page id</param>
        /// <param name="to">Tranlated language</param>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="400"> Returns if experiencing client issues</response>
        ///  <response code ="500"> Returns if experiencing server issues</response>
        ///  <response code ="404"> Returns if not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("{pageId}/translate")]
        public async Task<IActionResult> TranslatePage([ValidGuid("Invalid book id")] Guid bookId, [ValidGuid("Invalid page id")] Guid pageId, [Required]string to, CancellationToken cancellationToken)
        {
            var response = await Application.SendAsync(new TranslatePageRequest()
            {
                PageId = pageId,
                BookId = bookId,
                TranslatedLanguage = to,
                UserId = CurrentUser
            });
            return response.Response();
        }

    }
}
