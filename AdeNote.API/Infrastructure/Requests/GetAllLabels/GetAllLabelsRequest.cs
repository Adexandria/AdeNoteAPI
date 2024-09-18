using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.GetAllLabels
{
    public class GetAllLabelsRequest : IRequest<ActionResult<IEnumerable<LabelDTO>>>
    {
    }
}
