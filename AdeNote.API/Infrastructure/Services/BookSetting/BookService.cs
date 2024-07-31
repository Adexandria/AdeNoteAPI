using AdeCache.Services;
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
        private readonly ICacheService cacheService;

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
        /// <param name="_cacheService">Handles cache operations</param>
        public BookService(IBookRepository _bookRepository, ICacheService _cacheService)
        {
            bookRepository = _bookRepository;
            cacheService = _cacheService;
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

            List<Book> currentBooks;

            currentBooks = cacheService.Get<IEnumerable<Book>>(_cacheKey).ToList();

            if(currentBooks == null)
            {
                currentBooks = new List<Book>()
                {
                    book
                };

                cacheService.Set<IEnumerable<Book>>(_cacheKey, currentBooks, DateTime.UtcNow.AddMinutes(30));
            }
            else
            {
                currentBooks.Add(book);
                cacheService.Set<IEnumerable<Book>>(_cacheKey, currentBooks, DateTime.UtcNow.AddMinutes(30));
            }

            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Adds  new books
        /// </summary>
        /// <param name="userId">a user id</param>
        /// <param name="newBooks">an object to add new books</param>
        /// <returns>A custom action result</returns>
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

            List<Book> currentBooks;

            currentBooks = cacheService.Get<IEnumerable<Book>>(_cacheKey).ToList();

            if (currentBooks == null)
            {
                cacheService.Set<IEnumerable<Book>>(_cacheKey, books, DateTime.UtcNow.AddMinutes(30));
            }
            else
            {
                currentBooks.AddRange(books);
                cacheService.Set<IEnumerable<Book>>(_cacheKey, currentBooks, DateTime.UtcNow.AddMinutes(30));
            }

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

            var currentBooks = cacheService.Get<List<Book>>(_cacheKey);

            if (currentBooks == null)
            {
                currentBooks = bookRepository.GetAll(userId).ToList();
                cacheService.Set(_cacheKey, currentBooks, DateTime.UtcNow.AddMinutes(30));
            }

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

            Book currentBook;

            var currentBooks = cacheService.Get<IEnumerable<Book>>(_cacheKey);
           
            if(currentBooks != null)
            {
                currentBook = currentBooks.FirstOrDefault(s => s.Id == bookId && s.UserId == userId) ?? await bookRepository.GetAsync(bookId,userId);
            }
            else
            {
                currentBook = await bookRepository.GetAsync(bookId, userId);
            }

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

            Book currentBook;

            var currentBooks = cacheService.Get<IEnumerable<Book>>(_cacheKey).ToList();

            if(currentBooks != null)
            {
                currentBook =  currentBooks.FirstOrDefault(s => s.Id == bookId && s.UserId == userId) ?? await bookRepository.GetAsync(bookId, userId);
            }
            else
            {
                currentBook = await bookRepository.GetAsync(bookId, userId);
            }

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

            Book currentBook;

            var currentBooks = cacheService.Get<IEnumerable<Book>>(_cacheKey).ToList();

            if (currentBooks != null)
            {
                currentBook = currentBooks.FirstOrDefault(s => s.Id == bookId && s.UserId == userId) ?? await bookRepository.GetAsync(bookId, userId,true);
            }
            else
            {
                currentBook = await bookRepository.GetAsync(bookId, userId,true);
            }

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

        private readonly string _cacheKey = "Books";
    }
}
