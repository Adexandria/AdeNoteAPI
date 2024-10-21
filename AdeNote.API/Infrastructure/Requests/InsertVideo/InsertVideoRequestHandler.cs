using AdeAuth.Services.Extensions;
using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.InsertVideo
{
    public class InsertVideoRequestHandler : IRequestHandler<InsertVideoRequest, ActionResult<string>>
    {
        public InsertVideoRequestHandler(ICacheService cacheService, IPageRepository pageRepository,
            CachingKeys cachingKeys,
            IBlobService blobService, Cdn cdn, IVideoRepository videoRepository)
        {
            this.cacheService = cacheService;
            this.pageRepository = pageRepository;
            _pageCacheKey = cachingKeys.PageCacheKey;
            this.blobService = blobService;
            this.cdn = cdn;
            this.videoRepository = videoRepository;
        }

        public async Task<ActionResult<string>> Handle(InsertVideoRequest request, CancellationToken cancellationToken)
        {
            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{request.BookId}:{request.PageId}")
               ?? await pageRepository.GetBookPage(request.BookId, request.PageId);

            if (currentBookPage == null)
                return ActionResult<string>.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            var fileName = Guid.NewGuid().ToString()[^4..];

            _ = await blobService.UploadImage(fileName, request.Stream, mimeType: MimeType.mp4);

            var url = cdn.Endpoint + fileName +"." + MimeType.mp4.ToString();

            var video = new Video(request.Description, request.PageId, url);

            var commitStatus = await videoRepository.Add(video);

            if (!commitStatus)
                return ActionResult<string>.Failed("Failed to insert video");

            return ActionResult<string>.SuccessfulOperation(url);
        }
        
        private readonly ICacheService cacheService;
        private readonly IPageRepository pageRepository;
        private readonly IVideoRepository videoRepository; 
        private readonly Cdn cdn;
        private readonly IBlobService blobService;
        private readonly string _pageCacheKey;
    }
}
