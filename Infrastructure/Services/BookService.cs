using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Mapster;
using System.Net;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public class BookService : IBookService
    {
        public BookService(IBookRepository _bookRepository)
        {
            bookRepository = _bookRepository;
        }
        public async Task<ActionResult> Add(Guid userId, BookCreateDTO createBook)
        {
            try
            {
                var book = createBook.Adapt<Book>();
                var commitStatus = await bookRepository.Add(book);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to add new book");
                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
                
            }
          
        }

        public async Task<ActionResult<IEnumerable<BookDTO>>> GetAll(Guid userId)
        {
            var currentBooks = bookRepository.GetAll(userId);
            var currentBooksDTO = currentBooks.Adapt<IEnumerable<BookDTO>>();
            return await Task.FromResult(ActionResult<IEnumerable<BookDTO>>.SuccessfulOperation(currentBooksDTO));
        }

        public async Task<ActionResult<BookDTO>> GetById(Guid bookId,Guid userId)
        {
            try
            {
                if (bookId == Guid.Empty || userId == Guid.Empty)
                    return ActionResult<BookDTO>.Failed("Invalid id");

                var currentBook = await bookRepository.GetAsync(bookId,userId);

                if (currentBook == null)
                    return ActionResult<BookDTO>.Failed("Book does not exist", (int)HttpStatusCode.NotFound);

                var currentBookDTO = currentBook.Adapt<BookDTO>();

                return ActionResult<BookDTO>.SuccessfulOperation(currentBookDTO);
            }
            catch (Exception ex)
            {
                return ActionResult<BookDTO>.Failed(ex.Message);

            }
           
        }

        public async Task<ActionResult> Remove(Guid bookId, Guid userId)
        {
            try
            {
                if (bookId == Guid.Empty || userId == Guid.Empty)
                    return await Task.FromResult(ActionResult.Failed("Invalid id"));

                var currentBook = await bookRepository.GetAsync(bookId, userId);

                if (currentBook == null)
                    return ActionResult.Failed("Book does not exist", (int)HttpStatusCode.NotFound);

                var commitStatus = await bookRepository.Remove(currentBook);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to delete book");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);

            }
            
        }

        public async Task<ActionResult> Update(Guid bookId, Guid userId,BookUpdateDTO updateBook)
        {
            try
            {
                if (bookId == Guid.Empty || userId == Guid.Empty)
                    return await Task.FromResult(ActionResult.Failed("Invalid id"));

                var book = updateBook.Adapt<Book>();
                book.Id = bookId;
                book.UserId = userId;

                var currentBook = await bookRepository.GetAsync(bookId,userId);

                if (currentBook == null)
                    return ActionResult.Failed("Book does not exist", (int)HttpStatusCode.NotFound);

                var commitStatus = await bookRepository.Update(book);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to update book");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);

            }
        }

        private readonly IBookRepository bookRepository;
    }
}
