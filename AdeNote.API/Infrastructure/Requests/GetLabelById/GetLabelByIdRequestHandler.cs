using AdeCache.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.GetLabelById
{
    public class GetLabelByIdRequestHandler : IRequestHandler<GetLabelByIdRequest, ActionResult>
    {

        public GetLabelByIdRequestHandler(ILabelRepository _labelRepository,
                   ICacheService _cacheService, CachingKeys cachingKeys)
        {
            labelRepository = _labelRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.LabelCacheKey;
        }

        public async Task<ActionResult> Handle(GetLabelByIdRequest request, CancellationToken cancellationToken)
        {
            var currentLabel = cacheService.Get<Label>($"{_cacheKey}:{request.LabelId}");

            if (currentLabel == null)
            {
                currentLabel = await labelRepository.GetNoTrackingAsync(request.LabelId);
                cacheService.Set($"{_cacheKey}:{request.LabelId}", currentLabel);
            }

            if (currentLabel == null)
                return ActionResult<LabelDTO>.Failed("Label doesn't exist", (int)HttpStatusCode.NotFound);

            var currentLabelDTO = currentLabel.Map<Label, LabelDTO>(MappingService.LabelConfig());
            return ActionResult<LabelDTO>.SuccessfulOperation(currentLabelDTO);
        }

        private readonly ILabelRepository labelRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }
}
