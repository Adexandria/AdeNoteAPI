using AdeNote.Db;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class TicketRepository : Repository, ITicketRepository
    {
        public TicketRepository(NoteDbContext dbContext) :base(dbContext) 
        {
                
        }

        public async Task<bool> Add(Ticket entity)
        {
            entity.Id = Guid.NewGuid();

            await Db.Tickets.AddAsync(entity);

            return await SaveChanges<Ticket>();
        }

        public async Task<Ticket> GetTicket(Guid ticketId)
        {
           var ticket = await Db.Tickets
                .Where(s=>s.Id == ticketId)
                .Include(s=>s.User)
                .AsNoTracking().FirstOrDefaultAsync();

            if (ticket == null)
                return default;

            return ticket;
        }

        public IEnumerable<Ticket> GetTickets(int pageNumber, int pageSize)
        {
            var tickets = Db.Tickets
               .Include(s => s.User).OrderByDescending(s => s.Modified)
               .Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return tickets;
        }

        public IEnumerable<Ticket> GetTickets(Guid userId, int pageNumber, int pageSize)
        {
            var tickets = Db.Tickets
                .Where(s=>s.Id == userId)
              .Include(s => s.User).OrderByDescending(s=>s.Modified)
              .Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return tickets;
        }

        public async Task<bool> Remove(Ticket entity)
        {
            Db.Remove(entity);
            return await SaveChanges<Ticket>();
        }

        public IEnumerable<Ticket> SearchTickets(Func<Ticket, bool> expression, int pageNumber, int pageSize)
        {
            var tickets = Db.Tickets
                .Include(s => s.User)
                .AsNoTracking()
                 .Where(expression)
                 .OrderByDescending(s => s.Modified)
                 .Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return tickets;

        }

        public async Task<bool> Update(Ticket entity)
        {
            Db.Update(entity);

            return await SaveChanges<Ticket>();
        }
    }
}
