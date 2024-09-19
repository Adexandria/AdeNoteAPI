using AdeCache.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.GetPagesById
{
    public class GetPageByIdRequestHandler : IRequestHandler<GetPageByIdRequest, ActionResult<PageDTO>>
    {
        public GetPageByIdRequestHandler(IPageRepository _pageRepository,
            ICacheService _cacheService, CachingKeys cachingKeys)
        {
            pageRepository = _pageRepository;
            cacheService = _cacheService;
            _pageCacheKey = cachingKeys.PageCacheKey;
        }
        public async Task<ActionResult<PageDTO>> Handle(GetPageByIdRequest request, CancellationToken cancellationToken)
        {
            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{request.BookId}:{request.PageId}")
                ?? await pageRepository.GetBookPage(request.BookId, request.PageId);

            if (currentBookPage == null)
                return await Task.FromResult(ActionResult<PageDTO>.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));

            var currentBookPageDTO = currentBookPage.Map<Page, PageDTO>(MappingService.PageLabelsConfig());

            return ActionResult<PageDTO>.SuccessfulOperation(currentBookPageDTO);
        }

        private readonly IPageRepository pageRepository;
        private readonly ICacheService cacheService;
        private readonly string _pageCacheKey;
    }
}
