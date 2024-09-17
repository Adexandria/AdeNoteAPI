using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.GetBookdById
{
    public class GetBookByIdRequest : IRequest<ActionResult>
    {
        public Guid BookId { get; set; }

        public Guid UserId { get; set; }
    }
}
