using AdeAuth.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Db
{
    public class IdentityDbContext: IdentityContext<User>
    {
        public IdentityDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
