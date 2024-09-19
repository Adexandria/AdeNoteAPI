using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.CreatePageLabels
{
    public class CreatePageLabelsRequest : IRequest<ActionResult>
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
        public Guid PageId { get; set; }
        public List<string> Labels { get; set; }
    }
}
