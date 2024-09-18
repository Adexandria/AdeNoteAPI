using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.UpdateTicket
{
    public class UpdateTicketRequest : IRequest<ActionResult>
    {
        public string Status { get; set; }

        public Guid AdminId { get; set; }

        public Guid TicketId { get; set; }

        public SolvedTicketDto SolvedTicketDto {  get; set; }
    }
}
