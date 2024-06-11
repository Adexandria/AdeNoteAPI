using AdeNote.Db;
using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdeNote.Tests
{
    public class RepoTestHelpher<TRepo,TModel>
        where TRepo : Repository<TModel>
        where TModel : class
    {
        [SetUp]
        protected virtual void SetUp()
        {
            mockRepo = new Mock<TRepo>();
            Repo = mockRepo.Object;

            var dbOptions = new DbContextOptionsBuilder<NoteDbContext>()
                .UseInMemoryDatabase("NoteDb")
                .Options;

            Repo.Db = new NoteDbContext(dbOptions);

            Repo.Db.Database.EnsureCreated();

            obj = CreateModel();

            Repo.logger = new Mock<ILogger<TModel>>().Object;

        }

        public void AssumeSaveChangesSuccessfully()
        {
            mockRepo.Setup(s => s.SaveChanges()).ReturnsAsync(true);
        }

        public void AssumeSaveChangesFailed()
        {
            mockRepo.Setup(s => s.SaveChanges()).ReturnsAsync(false);
        }

        protected virtual TModel CreateModel()
        {
            return new Mock<TModel>().Object;
        }

        protected TModel obj { get; set; }
        protected TRepo Repo { get; set; }
        private Mock<TRepo> mockRepo { get; set; }

    }
}
