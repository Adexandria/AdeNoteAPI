using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.RemoveAllPageLabels
{
    public class RemoveAllPageLabelsRequest: IRequest<ActionResult>
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
        public Guid PageId { get; set; }
    }
}
