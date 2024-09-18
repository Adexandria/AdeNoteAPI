using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.FetchTicketById
{
    public class FetchTicketByIdRequest : IRequest<ActionResult<TicketDTO>>
    {
        public Guid TicketId { get; set; }
    }
}
