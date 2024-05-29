using AdeNote.Models;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Repository
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        IEnumerable<Ticket> GetTickets(int pageNumber, int pageSize);

        Task<Ticket> GetTicket(Guid ticketId);

        IEnumerable<Ticket> GetTickets(Guid userId,int pageNumber, int pageSize);

        IEnumerable<Ticket> SearchTickets(Func<Ticket, bool> expression, int pageNumber, int pageSize);
    }
}
