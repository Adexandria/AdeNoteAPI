using AdeAuth.Db;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Tests
{
    public class DbContextTestHelper
    {
        [SetUp]
        public virtual void Setup()
        {
           dbOptions = new DbContextOptionsBuilder<IdentityContext>()
               .UseInMemoryDatabase("IdentityDb")
               .Options;
        }

        protected DbContextOptions<IdentityContext> dbOptions { get; set; }
    }
}
