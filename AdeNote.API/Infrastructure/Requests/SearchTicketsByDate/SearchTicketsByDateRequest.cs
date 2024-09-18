using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.SearchTicketsByDate
{
    public class SearchTicketsByDateRequest: IRequest<ActionResult<PaginatedResponse<TicketsDTO>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Created { get; set; }
    }
}
