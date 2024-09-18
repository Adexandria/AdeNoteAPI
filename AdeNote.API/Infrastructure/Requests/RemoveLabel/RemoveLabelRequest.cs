using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.RemoveLabel
{
    public class RemoveLabelRequest : IRequest<ActionResult>
    {
        public Guid LabelId { get; set; }
    }
}
