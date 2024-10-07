using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using AdeNote.Models;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.RemovePage
{
    public class RemovePageRequestHandler : IRequestHandler<RemovePageRequest, ActionResult>
    {
        public RemovePageRequestHandler(IPageRepository pageRepository, 
            IBookRepository bookRepository, 
            ICacheService cacheService)
        {
            this.pageRepository = pageRepository;
            this.bookRepository = bookRepository;
            this.cacheService = cacheService;
        }
        public async Task<ActionResult> Handle(RemovePageRequest request, CancellationToken cancellationToken)
        {
            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{request.UserId}:{request.BookId}") 
                ?? await bookRepository.GetAsync(request.BookId, request.UserId);
            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{request.BookId}:{request.PageId}")
                ?? await pageRepository.GetBookPage(request.BookId, request.PageId, true);

            if (currentBookPage == null)
                return ActionResult<PageDTO>.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            var commitStatus = await pageRepository.Remove(currentBookPage);
            if (!commitStatus)
                return ActionResult.Failed("Failed to delete page");

            cacheService.Remove($"{_pageCacheKey}:{request.BookId}:{request.PageId}");

            return ActionResult.SuccessfulOperation();
        }

        private readonly ICacheService cacheService;
        private readonly IPageRepository pageRepository;
        private readonly IBookRepository bookRepository;
        private readonly string _pageCacheKey;
        private readonly string _bookCacheKey;
    }
}
