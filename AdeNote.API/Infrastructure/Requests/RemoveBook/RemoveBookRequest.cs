using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.RemoveBook
{
    public class RemoveBookRequest : IRequest<ActionResult>
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
    }
}
