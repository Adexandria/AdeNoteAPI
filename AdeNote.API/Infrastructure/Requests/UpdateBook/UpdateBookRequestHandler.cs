using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using AdeNote.Models;
using MediatR;
using System.Net;
using Automappify.Services;

namespace AdeNote.Infrastructure.Requests.UpdateBook
{
    public class UpdateBookRequestHandler : IRequestHandler<UpdateBookRequest, ActionResult>
    {
        public UpdateBookRequestHandler(IBookRepository _bookRepository,
            ICacheService _cacheService,
            CachingKeys cachingKeys)
        {
            bookRepository = _bookRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.BookCacheKey;
        }
        public async Task<ActionResult> Handle(UpdateBookRequest request, CancellationToken cancellationToken)
        {
            var book = request.UpdateBook.Map<BookUpdateDTO, Book>();

            book.Id = request.BookId;

            book.UserId = request.UserId;

            var currentBook = cacheService.Get<Book>($"{_cacheKey}:{request.UserId}:{request.BookId}") 
                ?? await bookRepository.GetAsync(request.BookId, request.UserId, true);

            if (currentBook == null)
                return ActionResult.Failed("Book does not exist", (int)HttpStatusCode.NotFound);

            book.SetModifiedDate();

            var commitStatus = await bookRepository.Update(book, currentBook);
            if (!commitStatus)
                return ActionResult.Failed("Failed to update book");

            cacheService.Set($"{_cacheKey}:{request.UserId}:{request.BookId}", book, DateTime.UtcNow.AddMinutes(30));

            return ActionResult.SuccessfulOperation();
        }

        private readonly IBookRepository bookRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }
}
