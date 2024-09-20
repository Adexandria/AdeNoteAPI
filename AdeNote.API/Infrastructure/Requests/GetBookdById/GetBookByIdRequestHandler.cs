using AdeCache.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models.DTOs;
using AdeNote.Models;
using MediatR;
using System.Net;
using Automappify.Services;

namespace AdeNote.Infrastructure.Requests.GetBookdById
{
    public class GetBookByIdRequestHandler : IRequestHandler<GetBookByIdRequest, ActionResult<BookDTO>>
    {

        public GetBookByIdRequestHandler(IBookRepository _bookRepository,
         ICacheService _cacheService,
         CachingKeys cachingKeys)
        {
            bookRepository = _bookRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.BookCacheKey;
        }

        public async Task<ActionResult<BookDTO>> Handle(GetBookByIdRequest request, CancellationToken cancellationToken)
        {
            var currentBook = cacheService.Get<Book>($"{_cacheKey}:{request.UserId}:{request.BookId}") 
                ?? await bookRepository.GetAsync(request.BookId, request.UserId);

            if (currentBook == null)
            {
                return ActionResult<BookDTO>.Failed("Book does not exist", (int)HttpStatusCode.NotFound);
            }

            cacheService.Set($"{_cacheKey}:{request.UserId}:{request.BookId}", 
                currentBook, DateTime.UtcNow.AddMinutes(30));

            var currentBookDTO = currentBook.Map<Book, BookDTO>(MappingService.BookConfig());

            return ActionResult<BookDTO>.SuccessfulOperation(currentBookDTO);
        }

        private readonly IBookRepository bookRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }

}
