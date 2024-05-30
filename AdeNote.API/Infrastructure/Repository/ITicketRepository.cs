using AdeNote.Models;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Repository
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        IEnumerable<Ticket> GetTickets(int pageNumber, int pageSize);

        Task<Ticket> GetTicket(Guid ticketId);

        int GetNumberOfTicketsByStatus(Status status);

        IEnumerable<Ticket> GetTickets(string name,int pageNumber, int pageSize);

        IEnumerable<Ticket> SearchTickets(Func<Ticket, bool> expression, int pageNumber, int pageSize);
    }
}
