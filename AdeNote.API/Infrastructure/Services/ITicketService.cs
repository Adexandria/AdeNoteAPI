using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services
{
    public interface ITicketService
    {
        Task<ActionResult> CreateTicket(TicketStreamDto newTicket, Guid userId);

        Task<ActionResult> UpdateTicket(string status, Guid adminId, Guid ticketId);

        Task<ActionResult> DeleteTicket(Guid ticketId);

        Task<ActionResult<TicketDTO>> FetchTicketById(Guid ticketId);

        ActionResult<PaginatedResponse<TicketsDTO>> FetchAllTickets(int pageNumber, int pageSize);

        ActionResult<PaginatedResponse<TicketsDTO>> FetchAllTickets(Guid userId, int pageNumber, int pageSize);
        ActionResult<PaginatedResponse<TicketsDTO>> SearchTickets(DateTime created, int pageNumber, int pageSize);
        ActionResult<PaginatedResponse<TicketsDTO>> SearchTickets(string status, int pageNumber, int pageSize);
    }
}
