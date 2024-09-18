using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.RemovePageLabel
{
    public class RemovePageLabelRequest : IRequest<ActionResult>
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
        public Guid PageId { get; set; }
        public string Title { get; set; }
    }
}
