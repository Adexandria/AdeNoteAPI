using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.InsertVideo
{
    public class InsertVideoRequest : IRequest<ActionResult<string>>
    {
        public Stream Stream { get; set; }
        public Guid PageId { get; set; }
        public Guid BookId { get; set; }
        public string Description { get; set; }
    }
}
