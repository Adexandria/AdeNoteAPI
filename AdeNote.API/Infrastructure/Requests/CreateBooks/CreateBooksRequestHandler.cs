using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models.DTOs;
using AdeNote.Models;
using MediatR;
using Automappify.Services;
using AdeCache.Services;

namespace AdeNote.Infrastructure.Requests.CreateBooks
{
    public class CreateBooksRequestHandler : IRequestHandler<CreateBooksRequest, ActionResult>
    {
        public CreateBooksRequestHandler(IBookRepository _bookRepository,
            ICacheService _cacheService, CachingKeys cachingKeys)
        {
            bookRepository = _bookRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.BookCacheKey;
        }

        public async Task<ActionResult> Handle(CreateBooksRequest request, CancellationToken cancellationToken)
        {
            var books = request.CreateBooks.Map<IList<BookCreateDTO>, IEnumerable<Book>>().ToList();
            for (int i = 0; i < books.Count; i++)
            {
                books[i].UserId = request.UserId;
                books[i].Id = Guid.NewGuid();
            }
            var commitStatus = await bookRepository.Add(books);
            if (!commitStatus)
                return ActionResult.Failed("Failed to add new book");

            books.ForEach(book => cacheService.Set($"{_cacheKey}:{book.UserId}:{book.Id}", book, DateTime.UtcNow.AddMinutes(30)));

            return ActionResult.SuccessfulOperation();
        }

        private readonly IBookRepository bookRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }
}
