using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.SearchTickets
{
    public class SearchTicketsRequest : IRequest<ActionResult<PaginatedResponse<TicketsDTO>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Status { get; set; }
    }
}
