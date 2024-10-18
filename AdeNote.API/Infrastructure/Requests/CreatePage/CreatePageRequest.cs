using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.CreatePage
{
    public class CreatePageRequest : IRequest<ActionResult>
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public Guid BookId { get; set; }

        public Guid UserId { get; set; }

        public Stream Stream { get; set; }

        public string Description { get; set; }
    }
}
