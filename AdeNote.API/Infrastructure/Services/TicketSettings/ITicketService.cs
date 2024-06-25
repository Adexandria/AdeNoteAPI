using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services.TicketSettings
{
    public interface ITicketService
    {
        Task<ActionResult> CreateTicket(TicketStreamDto newTicket,
           string email);

        Task<ActionResult> UpdateTicket(string status,
           Guid adminId,
             Guid ticketId);

        Task<ActionResult> DeleteTicket(Guid ticketId);

        Task<ActionResult<TicketDTO>> FetchTicketById(Guid ticketId);

        ActionResult<PaginatedResponse<TicketsDTO>> FetchAllTickets(int pageNumber, int pageSize);


        ActionResult<PaginatedResponse<TicketsDTO>> FetchAllTickets(string name, int pageNumber, int pageSize);

        ActionResult<PaginatedResponse<TicketsDTO>> SearchTicketsByDate(string created, int pageNumber, int pageSize);

        ActionResult<PaginatedResponse<TicketsDTO>> SearchTickets(string status, int pageNumber, int pageSize);
    }
}
