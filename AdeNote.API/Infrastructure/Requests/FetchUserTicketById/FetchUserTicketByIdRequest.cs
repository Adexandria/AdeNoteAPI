using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.FetchUserTicketById
{
    public class FetchUserTicketByIdRequest : IRequest<ActionResult<UserTicketDto>>
    {
        public string TicketId { get; set; }
    }
}
