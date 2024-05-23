using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IHangfireUserRepository :IRepository<HangfireUser>
    {
        public bool IsSeeded { get; }
    }
}
