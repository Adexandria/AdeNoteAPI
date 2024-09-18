using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using Automappify.Services;
using MediatR;

namespace AdeNote.Infrastructure.Requests.CreateLabel
{
    public class CreateLabelRequestHandler : IRequestHandler<CreateLabelRequest, ActionResult>
    {
        public CreateLabelRequestHandler(ILabelRepository _labelRepository,
                    ICacheService _cacheService, CachingKeys cachingKeys)
        {
            labelRepository = _labelRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.LabelCacheKey;
        }
        public async Task<ActionResult> Handle(CreateLabelRequest request, CancellationToken cancellationToken)
        {
            var label = request.Map<CreateLabelRequest, Label>();

            var commitStatus = await labelRepository.Add(label);
            if (!commitStatus)
                return ActionResult.Failed("Failed to add label");

            cacheService.Set($"{_cacheKey}:{label.Id}", label, DateTime.UtcNow.AddMinutes(30));

            return ActionResult.SuccessfulOperation();
        }

        private readonly ILabelRepository labelRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }
}
