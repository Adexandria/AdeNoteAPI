using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.CreatePageLabels
{
    public class CreatePageLabelRequestHandler : IRequestHandler<CreatePageLabelRequest, ActionResult>
    {
        public CreatePageLabelRequestHandler(IPageRepository pageRepository,
    IBookRepository bookRepository,
    ICacheService cacheService, ILabelPageRepository labelPageRepository,
    ILabelRepository labelRepository,
    CachingKeys cachingKeys)
        {
            this.pageRepository = pageRepository;
            this.bookRepository = bookRepository;
            this.cacheService = cacheService;
            _bookCacheKey = cachingKeys.BookCacheKey;
            _labelCacheKey = cachingKeys.LabelCacheKey;
            _pageCacheKey = cachingKeys.PageCacheKey;
            this.labelRepository = labelRepository;
            this.labelPageRepository = labelPageRepository;
        }

        public async Task<ActionResult> Handle(CreatePageLabelRequest request, CancellationToken cancellationToken)
        {
            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{request.UserId}:{request.BookId}") ?? await bookRepository.GetAsync(request.BookId, request.UserId);

            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{request.BookId}:{request.PageId}") ?? await pageRepository.GetBookPage(request.BookId, request.PageId);
            if (currentBookPage == null)
                return ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            if (request.Labels != null)
            {
                foreach (var label in request.Labels)
                {
                    var currentLabels = cacheService.Get<IEnumerable<Label>>(_labelCacheKey);

                    Label currentLabel = null;

                    if (currentLabels != null)
                    {
                        currentLabel = currentLabels.FirstOrDefault(s => s.Title == label);
                    }
                    else
                    {
                        currentLabel = await labelRepository.GetByNameAsync(label);
                    }

                    if (currentLabel == null)
                        return ActionResult.Failed("Label doesn't exist", StatusCodes.Status404NotFound);

                    if (currentBookPage.Labels != null)
                        if (currentBookPage.Labels.Any(s => s.Title == currentLabel.Title))
                        {
                            return ActionResult.Failed("Label has been added", (int)HttpStatusCode.BadRequest);
                        }

                    var status = await labelPageRepository.AddLabelToPage(request.PageId, currentLabel.Id);
                    if (!status)
                        return ActionResult.Failed("Failed to add label");
                }
            }

            return ActionResult.SuccessfulOperation();
        }

        private readonly ICacheService cacheService;
        private readonly IPageRepository pageRepository;
        private readonly IBookRepository bookRepository;
        private readonly ILabelRepository labelRepository;
        private readonly ILabelPageRepository labelPageRepository;
        private readonly string _pageCacheKey;
        private readonly string _bookCacheKey;
        private readonly string _labelCacheKey;
    }
}
