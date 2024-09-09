using AdeNote.Db;
using AdeNote.Infrastructure.Extension;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(NoteDbContext dbContext, IdentityDbContext identityDbContext, ILoggerFactory loggerFactory) :base(dbContext, loggerFactory) 
        {
            _identityDb = identityDbContext;
        }

        public async Task<bool> Add(Ticket entity)
        {
            entity.Id = Guid.NewGuid();

            await Db.Tickets.AddAsync(entity);

            var result = await SaveChanges();

            logger.LogInformation("Add ticket to database:{result}", result);

            return result;
        }

        public List<TicketStatusDto> GetNumberOfTicketsByStatus()
        {
            var tickets = Db.Tickets;

            var numberOfTickets = new List<TicketStatusDto>()
            {
                new(Status.Pending, tickets.Count(s=>s.Status == Status.Pending)),
                new(Status.Unresolved, tickets.Count(s=>s.Status == Status.Unresolved)),
                new(Status.Resolved, tickets.Count(s=>s.Status == Status.Resolved)),
                new(Status.Inreview, tickets.Count(s=>s.Status == Status.Inreview))

            };

            return numberOfTickets;
        }

        public async Task<Ticket> GetTicket(Guid ticketId)
        {
           var ticket = await Db.Tickets
                .Where(s=>s.Id == ticketId).Include(s=>s.User)
                .AsNoTracking().FirstOrDefaultAsync();


            return ticket;
        }

        public async Task<Ticket> GetTicket(string ticketId)
        {
            var id = ticketId.Remove(0, 2);
            var ticket = await Db.Tickets
                            .Where(s => s.Id.ToString().EndsWith(id)).Include(s => s.User)
                            .AsNoTracking().FirstOrDefaultAsync();

            return ticket;
        }

        public IEnumerable<Ticket> GetTickets(int pageNumber, int pageSize)
        {
            var tickets = Db.Tickets.OrderByDescending(s => s.Modified)
               .Skip((pageNumber - 1) * pageSize).Take(pageSize).Include(s=>s.User);

            return tickets;
        }

        public IEnumerable<Ticket> GetTickets(string name, int pageNumber, int pageSize)
        {

            var tickets = Db.Tickets.Include(s=>s.User)
                .Where(s => s.User.FirstName.StartsWith(name) || s.User.LastName.StartsWith(name))
              .OrderByDescending(s=>s.Modified)
              .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


            return tickets;
        }

        public async Task<bool> Remove(Ticket entity)
        {
            Db.Tickets.Remove(entity);

            var result = await SaveChanges();

            logger.LogInformation("Remove ticket to database:{result}", result);

            return result;
        }

        public IEnumerable<Ticket> SearchTickets(Func<Ticket, bool> expression, int pageNumber, int pageSize)
        {
            var tickets = Db.Tickets
                .Include(s=>s.User)
                .AsNoTracking()
                 .Where(expression)
                 .OrderByDescending(s => s.Modified)
                 .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return tickets;

        }

        public async Task<bool> Update(Ticket entity)
        {
            Db.Tickets.Update(entity);

            var result = await SaveChanges();

            logger.LogInformation("Update ticket to database:{result}", result);

            return result;
        }

        private readonly IdentityDbContext _identityDb;
    }
}
