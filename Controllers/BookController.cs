﻿using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    /// <summary>
    /// Handles all the end points for book.
    /// User can create,update,fetch and delete books
    /// </summary>
    [Route("api/books")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="bookService">An interface that interacts with the book tables</param>
        /// <param name="userIdentity">An interface that interacts with the user.
        /// This fetches the current user details</param>
        public BookController(IBookService bookService, IUserIdentity userIdentity) : base(userIdentity)
        {
            _bookService = bookService;

        }


        /// <summary>
        /// fetches all books
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             GET /books
        ///             
        /// </remarks>
        /// <returns> A list of books</returns>
        /// <response code ="200"> Returns if successful</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<IEnumerable<BookDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var response = await _bookService.GetAll(CurrentUser);
            return response.Response();
        }

        /// <summary>
        /// Fetches a particular book
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             GET /books/20b1204e-fad5-4a90-a78e-bc3b988afd60
        /// </remarks>
        /// <param name="bookId">A guid format of a book id</param>
        /// <returns>A book</returns>
        /// <response code ="200"> Returns if successful</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <response code ="404"> Returns book not found</response>
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<BookDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetBook(Guid bookId)
        {
            var response = await _bookService.GetById(bookId, CurrentUser);
            return response.Response();
        }

        /// <summary>
        /// Adds a new book
        /// </summary>
        /// <remarks>
        ///Sample request:
        ///     
        ///             POST /books
        ///             {
        ///                 "title": "string",
        ///                 "description": "string"
        ///             }
        /// </remarks>
        /// <param name="createBook">An object used to add a new book</param>
        /// <response code ="200"> Returns if book was created</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public async Task<IActionResult> CreateBook(BookCreateDTO createBook)
        {
            var response = await _bookService.Add(CurrentUser, createBook);
            return response.Response();
        }

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <remarks>
        ///  Sample request:
        ///                 
        ///                 PUT /books/20b1204e-fad5-4a90-a78e-bc3b988afd60
        ///                 {
        ///                     "title": "string",
        ///                     "description": "string"
        ///                 }
        /// </remarks>
        /// <param name="bookId">A guid format of a book id</param>
        /// <param name="bookUpdate">A model to update an existing book</param>
        /// <response code ="200"> Returns if book was updated</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <response code ="404"> Returns if book not found</response>
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPut("{bookId}")]
        public async Task<IActionResult> UpdateBook(Guid bookId,BookUpdateDTO bookUpdate)
        {
            var response = await _bookService.Update(bookId,CurrentUser,bookUpdate);
            return response.Response();
        }

        /// <summary>
        /// Deletes an existing book
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             DELETE  /books/20b1204e-fad5-4a90-a78e-bc3b988afd60
        ///         
        /// </remarks>
        /// <param name="bookId">A guid format of a book id</param>
        /// <response code ="200"> Returns if book was deleted</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <response code ="404"> Returns if book was not found</response>
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            var response = await _bookService.Remove(bookId, CurrentUser);
            return response.Response();
        }
    }
}
