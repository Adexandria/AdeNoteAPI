using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.FetchAllTickets
{
    public class FetchAllTicketsRequest : IRequest<ActionResult<PaginatedResponse<TicketsDTO>>>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
