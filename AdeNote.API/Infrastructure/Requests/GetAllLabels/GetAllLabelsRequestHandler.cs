using AdeCache.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using MediatR;

namespace AdeNote.Infrastructure.Requests.GetAllLabels
{
    public class GetAllLabelsRequestHandler : IRequestHandler<GetAllLabelsRequest, ActionResult<IEnumerable<LabelDTO>>>
    {
        public GetAllLabelsRequestHandler(ILabelRepository _labelRepository,
            ICacheService _cacheService, CachingKeys cachingKeys)
        {
            labelRepository = _labelRepository;
            cacheService = _cacheService;
            _cacheKey = cachingKeys.LabelCacheKey;
        }

        public async Task<ActionResult<IEnumerable<LabelDTO>>> Handle(GetAllLabelsRequest request, CancellationToken cancellationToken)
        {
            var currentLabels = cacheService.Search<Label>(_cacheKey, "*");

            if (currentLabels == null)
            {
                currentLabels = labelRepository.GetAll().ToList();
                currentLabels.Foreach(label => cacheService.Set($"{_cacheKey}:{label.Id}", label, DateTime.UtcNow.AddMinutes(30)));
            }

            var currentLabelsDTO = currentLabels.Map<IEnumerable<Label>, IEnumerable<LabelDTO>>(MappingService.LabelConfig());

            return ActionResult<IEnumerable<LabelDTO>>.SuccessfulOperation(currentLabelsDTO);
        }

        private readonly ILabelRepository labelRepository;

        private readonly ICacheService cacheService;

        private readonly string _cacheKey;
    }
}
