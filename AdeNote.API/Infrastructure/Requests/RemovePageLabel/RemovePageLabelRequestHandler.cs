using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.RemovePageLabel
{
    public class RemovePageLabelRequestHandler : IRequestHandler<RemovePageLabelRequest, ActionResult>
    {
        public RemovePageLabelRequestHandler(IPageRepository pageRepository,
            IBookRepository bookRepository,
            ICacheService cacheService, ILabelPageRepository labelPageRepository,
            CachingKeys cachingKeys)
        {
            this.pageRepository = pageRepository;
            this.bookRepository = bookRepository;
            this.cacheService = cacheService;
            _bookCacheKey = cachingKeys.BookCacheKey;
            _pageCacheKey = cachingKeys.PageCacheKey;
            this.labelPageRepository = labelPageRepository;
        }
        public async Task<ActionResult> Handle(RemovePageLabelRequest request, CancellationToken cancellationToken)
        {
            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{request.UserId}:{request.BookId}") 
                ?? await bookRepository.GetAsync(request.BookId, request.UserId);
            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{request.BookId}:{request.PageId}")
                ?? await pageRepository.GetBookPage(request.BookId, request.PageId);
            if (currentBookPage == null)
                return ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            var currentLabel = currentBookPage.Labels.Where(s => s.Title == request.Title)
                .Select(s => s.Id).FirstOrDefault();
            if (currentLabel == Guid.Empty)
                return ActionResult.Failed("Label doesn't exist in this page", (int)HttpStatusCode.NotFound);

            var currentLabelPage = await labelPageRepository.GetLabel(request.PageId, currentLabel);
            if (currentLabelPage == null)
                return ActionResult.Failed("Label doesn't exist in this page", (int)HttpStatusCode.NotFound);

            var commitStatus = await labelPageRepository.DeleteLabelFromPage(currentLabelPage);

            if (!commitStatus)
                return ActionResult.Failed("Failed to delete label");

            return ActionResult.SuccessfulOperation();
        }

        private readonly ICacheService cacheService;
        private readonly IPageRepository pageRepository;
        private readonly IBookRepository bookRepository;
        private readonly ILabelPageRepository labelPageRepository;
        private readonly string _pageCacheKey;
        private readonly string _bookCacheKey;
    }
}
