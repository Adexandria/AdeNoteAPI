using AdeCache.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.GetAllBooks
{
    public class GetAllBooksRequestHandler : IRequestHandler<GetAllBooksRequest, ActionResult<IEnumerable<BookDTO>>>
    {
        public GetAllBooksRequestHandler(IBookRepository _bookRepository,
            ICacheService _cacheService,
            CachingKeys cachingKeys)
        {
            bookRepository = _bookRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.BookCacheKey;
        }

        public async Task<ActionResult<IEnumerable<BookDTO>>> Handle(GetAllBooksRequest request, CancellationToken cancellationToken)
        {
            var currentBooks = cacheService.Search<Book>(_cacheKey, "*");

            if (currentBooks == null)
            {
                currentBooks = bookRepository.GetAll(request.UserId).ToList();

                currentBooks.Foreach(book => cacheService.Set($"{_cacheKey}:{book.UserId}:{book.Id}", book, DateTime.UtcNow.AddMinutes(30)));
            }

            var currentBooksDTO = currentBooks.Map<IEnumerable<Book>, IEnumerable<BookDTO>>(MappingService.BookConfig());

            return await Task.FromResult(ActionResult<IEnumerable<BookDTO>>.SuccessfulOperation(currentBooksDTO));
        }

        private readonly IBookRepository bookRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }
}
