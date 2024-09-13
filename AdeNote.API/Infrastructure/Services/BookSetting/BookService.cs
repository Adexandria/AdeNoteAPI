using AdeCache.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using System.Net;

namespace AdeNote.Infrastructure.Services.BookSetting
{
    /// <summary>
    /// Implementation of the interface
    /// </summary>
    public class BookService : IBookService
    {
        public ICacheService cacheService;

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
        /// <param name="cachingKeys">Handles caching keys</param>
        public BookService(IBookRepository _bookRepository, 
            ICacheService _cacheService, CachingKeys cachingKeys)
        {
            bookRepository = _bookRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.BookCacheKey;
        }

        /// <summary>
        /// Adds a new book
        /// </summary>
        /// <param name="userId">a user id</param>
        /// <param name="createBook">an object to add a new book</param>
        /// <returns>A custom action result</returns>
        public async Task<ActionResult> Add(Guid userId, BookCreateDTO createBook)
        {
            var book = createBook.Map<BookCreateDTO,Book>();
            book.UserId = userId;

            var commitStatus = await bookRepository.Add(book);

            if (!commitStatus)
                return ActionResult.Failed("Failed to add new book");

            cacheService.Set($"{_cacheKey}:{book.UserId}:{book.Id}", book, DateTime.UtcNow.AddMinutes(30));

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
            var books = newBooks.Map<IList<BookCreateDTO>,IEnumerable<Book>>().ToList();
            for (int i = 0; i < books.Count; i++)
            {
                books[i].UserId = userId;
                books[i].Id = Guid.NewGuid();
            }
            var commitStatus = await bookRepository.Add(books);
            if (!commitStatus)
                return ActionResult.Failed("Failed to add new book");

            books.ForEach(book => cacheService.Set($"{_cacheKey}:{book.UserId}:{book.Id}", book, DateTime.UtcNow.AddMinutes(30)));

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

            var currentBooks = cacheService.Search<Book>(_cacheKey,"*");

            if(currentBooks == null)
            {
                currentBooks = bookRepository.GetAll(userId).ToList();

                currentBooks.Foreach(book => cacheService.Set($"{_cacheKey}:{book.UserId}:{book.Id}", book, DateTime.UtcNow.AddMinutes(30)));
            }

            var currentBooksDTO = currentBooks.Map<IEnumerable<Book>,IEnumerable<BookDTO>>(MappingService.BookConfig());

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

            var currentBook = cacheService.Get<Book>($"{_cacheKey}:{userId}:{bookId}") ?? await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
            {
                return ActionResult<BookDTO>.Failed("Book does not exist", (int)HttpStatusCode.NotFound);
            }

            cacheService.Set($"{_cacheKey}:{userId}:{bookId}", currentBook, DateTime.UtcNow.AddMinutes(30));
            var currentBookDTO = currentBook.Map<Book,BookDTO>(MappingService.BookConfig());

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

            var currentBook = cacheService.Get<Book>($"{_cacheKey}:{userId}:{bookId}") ?? await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
                return ActionResult<BookDTO>.Failed("Book does not exist", (int)HttpStatusCode.NotFound);

            var commitStatus = await bookRepository.Remove(currentBook);
            if (!commitStatus)
                return ActionResult.Failed("Failed to delete book");


            cacheService.Remove($"{_cacheKey}:{userId}:{bookId}");

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

            var book = updateBook.Map<BookUpdateDTO,Book>();
            book.Id = bookId;
            book.UserId = userId;

            var currentBook = cacheService.Get<Book>($"{_cacheKey}:{userId}:{bookId}") ?? await bookRepository.GetAsync(bookId, userId,true);

            if (currentBook == null)
                return ActionResult.Failed("Book does not exist", (int)HttpStatusCode.NotFound);

            book.SetModifiedDate();

            var commitStatus = await bookRepository.Update(book, currentBook);
            if (!commitStatus)
                return ActionResult.Failed("Failed to update book");

            cacheService.Set($"{_cacheKey}:{userId}:{bookId}", book, DateTime.UtcNow.AddMinutes(30));

            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Handles the persisting and querying of book object
        /// </summary>
        public IBookRepository bookRepository { get; set; }

        private readonly string _cacheKey;
    }
}
