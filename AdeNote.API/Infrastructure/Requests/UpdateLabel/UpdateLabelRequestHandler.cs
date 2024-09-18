using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.UpdateLabel
{
    public class UpdateLabelRequestHandler : IRequestHandler<UpdateLabelRequest, ActionResult>
    {

        public UpdateLabelRequestHandler(ILabelRepository _labelRepository,
            ICacheService _cacheService, CachingKeys cachingKeys)
        {
            labelRepository = _labelRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.LabelCacheKey;
        }

        public async Task<ActionResult> Handle(UpdateLabelRequest request, CancellationToken cancellationToken)
        {
            var currentLabel = cacheService.Get<Label>($"{_cacheKey}:{request.LabelId}") 
                ?? await labelRepository.GetAsync(request.LabelId);
            if (currentLabel == null)
                return ActionResult.Failed("label doesn't exist", (int)HttpStatusCode.NotFound);

            var label = request.UpdateLabel.Map<LabelUpdateDTO, Label>();

            label.Id = request.LabelId;

            label.SetModifiedDate();

            var commitStatus = await labelRepository.Update(label, currentLabel);
            if (!commitStatus)
                return ActionResult.Failed("Failed to update label");

            cacheService.Set(_cacheKey, label);

            return ActionResult.SuccessfulOperation();
        }

        private readonly ILabelRepository labelRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }
}
