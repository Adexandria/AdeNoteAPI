using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using MediatR;
using AdeNote.Infrastructure.Repository;
using AdeCache.Services;

namespace AdeNote.Infrastructure.Requests.CreateBook
{
    public class CreateBookRequestHandler : IRequestHandler<CreateBookRequest, ActionResult>
    {
        public CreateBookRequestHandler(IBookRepository _bookRepository, 
            ICacheService _cacheService, CachingKeys cachingKeys)
        {
            bookRepository = _bookRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.BookCacheKey;
        }


        public async Task<ActionResult> Handle(CreateBookRequest request, CancellationToken cancellationToken)
        {
            var book = new Book(request.Title, request.Description).SetUser(request.UserId);

            var commitStatus = await bookRepository.Add(book);

            if (!commitStatus)
                return ActionResult.Failed("Failed to add new book");

            cacheService.Set($"{_cacheKey}:{book.UserId}:{book.Id}", book, DateTime.UtcNow.AddMinutes(30));

            return ActionResult.SuccessfulOperation();
        }


        private readonly IBookRepository bookRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }
}
