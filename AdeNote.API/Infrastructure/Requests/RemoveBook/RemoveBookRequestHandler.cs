using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models.DTOs;
using AdeNote.Models;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.RemoveBook
{
    public class RemoveBookRequestHandler : IRequestHandler<RemoveBookRequest, ActionResult>
    {
        public RemoveBookRequestHandler(IBookRepository _bookRepository,
            ICacheService _cacheService,
            CachingKeys cachingKeys)
        {
            bookRepository = _bookRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.BookCacheKey;
        }
        public async Task<ActionResult> Handle(RemoveBookRequest request, CancellationToken cancellationToken)
        {
           var currentBook = cacheService.Get<Book>($"{_cacheKey}:{request.UserId}:{request.BookId}") 
                ?? await bookRepository.GetAsync(request.BookId, request.UserId);

            if (currentBook == null)
                return ActionResult<BookDTO>.Failed("Book does not exist", (int)HttpStatusCode.NotFound);

            var commitStatus = await bookRepository.Remove(currentBook);
            if (!commitStatus)
                return ActionResult.Failed("Failed to delete book");


            cacheService.Remove($"{_cacheKey}:{request.UserId}:{request.BookId}");

            return ActionResult.SuccessfulOperation();
        }

        private readonly IBookRepository bookRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }
}
