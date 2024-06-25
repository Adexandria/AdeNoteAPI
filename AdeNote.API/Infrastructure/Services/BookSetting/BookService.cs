using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Mapster;
using System.Net;

namespace AdeNote.Infrastructure.Services.BookSetting
{
    /// <summary>
    /// Implementation of the interface
    /// </summary>
    public class BookService : IBookService
    {
        /// <summary>
        /// A Constructor
        /// </summary>
        protected BookService()
        {

        }
        /// <summary>
        /// A Constructor
        /// </summary>
        /// <param name="_bookRepository">Handles the persisting and querying of book object</param>
        public BookService(IBookRepository _bookRepository)
        {
            bookRepository = _bookRepository;
        }

        /// <summary>
        /// Adds a new book
        /// </summary>
        /// <param name="userId">a user id</param>
        /// <param name="createBook">an object to add a new book</param>
        /// <returns>A custom action result</returns>
        public async Task<ActionResult> Add(Guid userId, BookCreateDTO createBook)
        {
            var book = createBook.Adapt<Book>();
            book.UserId = userId;


            var commitStatus = await bookRepository.Add(book);

            if (!commitStatus)
                return ActionResult.Failed("Failed to add new book");

            return ActionResult.SuccessfulOperation();


        }

        public async Task<ActionResult> Add(Guid userId, IList<BookCreateDTO> newBooks)
        {
            var books = newBooks.Adapt<IEnumerable<Book>>().ToList();
            for (int i = 0; i < books.Count(); i++)
            {
                books[i].UserId = userId;
                books[i].Id = Guid.NewGuid();
            }
            var commitStatus = await bookRepository.Add(books);
            if (!commitStatus)
                return ActionResult.Failed("Failed to add new book");
            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Get all books
        /// </summary>
        /// <param name="userId">a user id</param>
        /// <returns>A custom action result</returns>
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetAll(Guid userId)
        {
            if (userId == Guid.Empty)
                return ActionResult<IEnumerable<BookDTO>>.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

            var currentBooks = bookRepository.GetAll(userId);
            var currentBooksDTO = currentBooks.Adapt<IEnumerable<BookDTO>>(MappingService.BookConfig());
            return await Task.FromResult(ActionResult<IEnumerable<BookDTO>>.SuccessfulOperation(currentBooksDTO));
        }

        /// <summary>
        /// Get book by id
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="userId">a user id</param>
        /// <returns>A custom action result</returns>
        public async Task<ActionResult<BookDTO>> GetById(Guid bookId, Guid userId)
        {

            if (userId == Guid.Empty)
                return ActionResult<BookDTO>.Failed("Invalid user id", (int)HttpStatusCode.BadRequest);


            var currentBook = await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
                return ActionResult<BookDTO>.Failed("Book does not exist", (int)HttpStatusCode.NotFound);

            var currentBookDTO = currentBook.Adapt<BookDTO>();

            return ActionResult<BookDTO>.SuccessfulOperation(currentBookDTO);

        }

        /// <summary>
        /// Deletes an existing book
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="userId">a user id</param>
        /// <returns>A custom action result</returns>
        public async Task<ActionResult> Remove(Guid bookId, Guid userId)
        {

            if (bookId == Guid.Empty || userId == Guid.Empty)
                return await Task.FromResult(ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest));

            var currentBook = await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
                return ActionResult.Failed("Book does not exist", (int)HttpStatusCode.NotFound);

            var commitStatus = await bookRepository.Remove(currentBook);
            if (!commitStatus)
                return ActionResult.Failed("Failed to delete book");

            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="userId">a user id</param>
        /// <param name="updateBook">An object to update a book</param>
        /// <returns>A custom action result</returns>
        public async Task<ActionResult> Update(Guid bookId, Guid userId, BookUpdateDTO updateBook)
        {

            if (bookId == Guid.Empty || userId == Guid.Empty)
                return await Task.FromResult(ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest));

            var book = updateBook.Adapt<Book>();
            book.Id = bookId;
            book.UserId = userId;

            var currentBook = await bookRepository.GetAsync(bookId, userId, true);

            if (currentBook == null)
                return ActionResult.Failed("Book does not exist", (int)HttpStatusCode.NotFound);

            book.SetModifiedDate();

            var commitStatus = await bookRepository.Update(book, currentBook);
            if (!commitStatus)
                return ActionResult.Failed("Failed to update book");

            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Handles the persisting and querying of book object
        /// </summary>
        public IBookRepository bookRepository { get; set; }
    }
}
