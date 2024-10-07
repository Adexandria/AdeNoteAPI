using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models.DTOs;
using AdeNote.Models;
using MediatR;
using System.Net;
using Automappify.Services;

namespace AdeNote.Infrastructure.Requests.UpdatePage
{
    public class UpdatePageRequestHandler : IRequestHandler<UpdatePageRequest, ActionResult>
    {
        public UpdatePageRequestHandler(IPageRepository pageRepository,
          IBookRepository bookRepository,
          ICacheService cacheService)
        {
            this.pageRepository = pageRepository;
            this.bookRepository = bookRepository;
            this.cacheService = cacheService;
        }
        public async Task<ActionResult> Handle(UpdatePageRequest request, CancellationToken cancellationToken)
        {
            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{request.UserId}:{request.BookId}") 
                ?? await bookRepository.GetAsync(request.BookId, request.UserId);

            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{request.BookId}:{request.PageId}") 
                ?? await pageRepository.GetBookPage(request.BookId, request.PageId, true);
            if (currentBookPage == null)
                return ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            var page = request.UpdatePage.Map<PageUpdateDTO, Page>();

            page.Id = request.PageId;
            page.BookId = request.BookId;

            page.SetModifiedDate();

            var commitStatus = await pageRepository.Update(page, currentBookPage);
            if (!commitStatus)
                return ActionResult.Failed("Failed to update page");

            cacheService.Set($"{_pageCacheKey}:{request.BookId}:{request.PageId}", page, DateTime.UtcNow.AddMinutes(30));

            return ActionResult.SuccessfulOperation();
        }

        private readonly ICacheService cacheService;
        private readonly IPageRepository pageRepository;
        private readonly IBookRepository bookRepository;
        private readonly string _pageCacheKey;
        private readonly string _bookCacheKey;
    }
}
