using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.GetBookdById
{
    public class GetBookByIdRequest : IRequest<ActionResult<BookDTO>>
    {
        public Guid BookId { get; set; }

        public Guid UserId { get; set; }
    }
}
