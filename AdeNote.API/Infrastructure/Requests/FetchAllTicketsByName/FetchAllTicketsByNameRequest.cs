using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.FetchAllTicketsByName
{
    public class FetchAllTicketsByNameRequest : IRequest<ActionResult<PaginatedResponse<TicketsDTO>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Name { get; set; }
    }
}
