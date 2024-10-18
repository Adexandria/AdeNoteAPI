using AdeCache.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using MediatR;

namespace AdeNote.Infrastructure.Requests.GetAllPages
{
    public class GetAllPagesRequestHandler : IRequestHandler<GetAllPagesRequest, ActionResult<IEnumerable<PageDTO>>>
    {
        public GetAllPagesRequestHandler(IPageRepository _pageRepository,
    ICacheService _cacheService, CachingKeys cachingKeys)
        {
            pageRepository = _pageRepository;
            cacheService = _cacheService;
            _pageCacheKey = cachingKeys.PageCacheKey;
        }

        public async Task<ActionResult<IEnumerable<PageDTO>>> Handle(GetAllPagesRequest request, CancellationToken cancellationToken)
        {
            var currentPages = cacheService.Search<Page>(_pageCacheKey, "*");

            if (currentPages == null)
            {
                currentPages = pageRepository.GetBookPages(request.BookId).ToList();
                currentPages.Foreach(currentPage => cacheService.Set($"{_pageCacheKey}:{request.BookId}:{currentPage.Id}", currentPage, DateTime.UtcNow.AddMinutes(30)));
            }

            var currentBookPagesDTO = currentPages.Map<IEnumerable<Page>, IEnumerable<PageDTO>>(MappingService.PageLabelsConfig());

            return ActionResult<IEnumerable<PageDTO>>.SuccessfulOperation(currentBookPagesDTO);
        }

        private readonly IPageRepository pageRepository;
        private readonly ICacheService cacheService;
        private readonly string _pageCacheKey;
    }
}
