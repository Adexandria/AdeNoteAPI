using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.RemoveTicket
{
    public class RemoveTicketRequest : IRequest<ActionResult>
    {
        public Guid TicketId { get; set; }
    }
}
