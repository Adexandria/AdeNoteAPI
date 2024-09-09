using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services.TicketSettings
{
    public interface ITicketService
    {
        Task<ActionResult> CreateTicket(TicketStreamDto newTicket,
           string email, CancellationToken cancellationToken = default);

        Task<ActionResult> UpdateTicket(string status, Guid adminId,Guid ticketId, SolvedTicketDto solvedTicketDto);

        Task<ActionResult> DeleteTicket(Guid ticketId);

        Task<ActionResult<TicketDTO>> FetchTicketById(Guid ticketId);

        Task<ActionResult<UserTicketDto>> FetchTicketById(string ticketId);

        ActionResult<PaginatedResponse<TicketsDTO>> FetchAllTickets(int pageNumber, int pageSize);


        ActionResult<PaginatedResponse<TicketsDTO>> FetchAllTickets(string name, int pageNumber, int pageSize);

        ActionResult<PaginatedResponse<TicketsDTO>> SearchTicketsByDate(string created, int pageNumber, int pageSize);

        ActionResult<PaginatedResponse<TicketsDTO>> SearchTickets(string status, int pageNumber, int pageSize);
    }
}
