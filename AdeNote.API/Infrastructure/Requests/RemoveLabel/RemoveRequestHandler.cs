using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.RemoveLabel
{
    public class RemoveRequestHandler : IRequestHandler<RemoveLabelRequest, ActionResult>
    {
        public RemoveRequestHandler(ILabelRepository _labelRepository,
            ICacheService _cacheService, CachingKeys cachingKeys)
        {
            labelRepository = _labelRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.LabelCacheKey;
        }
        public async Task<ActionResult> Handle(RemoveLabelRequest request, CancellationToken cancellationToken)
        {
            var currentLabel = cacheService.Get<Label>($"{_cacheKey}:{request.LabelId}")
                ?? await labelRepository.GetNoTrackingAsync(request.LabelId);
            if (currentLabel == null)
                return ActionResult.Failed("label doesn't exist", (int)HttpStatusCode.NotFound);

            var commitStatus = await labelRepository.Remove(currentLabel);
            if (!commitStatus)
                return ActionResult.Failed("Failed to remove label");

            cacheService.Remove(_cacheKey);

            return ActionResult.SuccessfulOperation();
        }

        private readonly ILabelRepository labelRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }
}
