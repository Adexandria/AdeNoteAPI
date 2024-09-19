using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.TranslationAI;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using MediatR;
using System.Net;
using Automappify.Services;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Requests.CreatePage
{
    public class CreatePageRequestHandler : IRequestHandler<CreatePageRequest, ActionResult>
    {
        public CreatePageRequestHandler(IPageRepository _pageRepository,
            IBookRepository _bookRepository,
            ICacheService _cacheService, CachingKeys cachingKeys)
        {
            pageRepository = _pageRepository;
            bookRepository = _bookRepository;
            cacheService = _cacheService;
            _bookCacheKey = cachingKeys.BookCacheKey;
            _pageCacheKey = cachingKeys.PageCacheKey;
        }
        public async Task<ActionResult> Handle(CreatePageRequest request, CancellationToken cancellationToken)
        {
            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{request.UserId}:{request.BookId}") ?? await bookRepository
                .GetAsync(request.BookId, request.UserId);

            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var page = request.Map<CreatePageRequest,Page>();

            page.BookId = request.BookId;

            var commitStatus = await pageRepository.Add(page);
            if (!commitStatus)
                return ActionResult.Failed("Failed to add page");

            cacheService.Set($"{_pageCacheKey}:{request.BookId}:{page.Id}", page, DateTime.UtcNow.AddMinutes(30));

            return ActionResult.SuccessfulOperation();
        }

        private readonly IPageRepository pageRepository;
        private readonly IBookRepository bookRepository;
        private readonly ICacheService cacheService;
        private readonly string _bookCacheKey;
        private readonly string _pageCacheKey;
    }
}
