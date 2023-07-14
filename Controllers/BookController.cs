using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    [Route("api/books")]
    [ApiController]
    [Authorize]
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService, IUserIdentity userIdentity) : base(userIdentity)
        {
            _bookService = bookService;

        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var response = await _bookService.GetAll(CurrentUser);
            return response.Response();
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetBook(Guid bookId)
        {
            var response = await _bookService.GetById(bookId, CurrentUser);
            return response.Response();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(BookCreateDTO createBook)
        {
            var response = await _bookService.Add(CurrentUser,createBook);
            return response.Response();
        }

        [HttpPut("{bookId}")]
        public async Task<IActionResult> UpdateBook(Guid bookId,BookUpdateDTO bookUpdate)
        {
            var response = await _bookService.Update(bookId,CurrentUser,bookUpdate);
            return response.Response();
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            var response = await _bookService.Remove(bookId, CurrentUser);
            return response.Response();
        }
    }
}
