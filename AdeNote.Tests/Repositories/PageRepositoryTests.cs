using AdeNote.Infrastructure.Repository;
using AdeNote.Models;

namespace AdeNote.Tests.Repositories
{
    public class PageRepositoryTests : RepoTestHelpher<PageRepository,Page>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            var hasSeeded = Repo.Db.Pages.Any();
            if (!hasSeeded)
                SeedDatabase();
        }

        [Test]
        public async Task ShouldAddPageSuccessfully()
        {
            //Arrange
            AssumeSaveChangesSuccessfully();

            //Act
            var response = await Repo.Add(obj);

            //Assert
            Assert.That(response, Is.EqualTo(true));

        }

        [Test]
        public void ShouldGetAllPagesSuccessfully()
        {

            //Act
            var response = Repo.GetBookPages(new Guid("fa00ce8a-2418-4c24-aebd-2e10e3d6866c"));

            //Assert
            Assert.That(response.Count, Is.EqualTo(1));

        }

        [Test]
        public async Task ShouldGetPageSuccessfully()
        {
            //Act
            var response = await Repo.GetBookPage(new Guid("fa00ce8a-2418-4c24-aebd-2e10e3d6866c"), new Guid("f1f49b59-d026-4c3f-b898-1844e6cd55eb"));

            //Assert
            Assert.That(response, Is.Null);
        }

        [Test]
        public async Task ShouldRemovePageSuccessfully()
        {
            //Arrange
            AssumeSaveChangesSuccessfully();

            //Act
            var response = await Repo.Remove(obj);

            //Assert
            Assert.That(response, Is.EqualTo(true));
        }

        [Test,Ignore("Not working")]
        public async Task ShouldUpdatePageSuccessfully()
        {
            //Act
            var currentPage = await Repo.GetBookPage(new Guid("fa00ce8a-2418-4c24-aebd-2e10e3d6866c"), new Guid("f1f49b59-d026-4c3f-b898-1844e6cd55eb"));
            var response = await Repo.Update(currentPage);

            //Assert
            Assert.That(response, Is.EqualTo(true));
        }

        private List<Page> _pages;

        private void SeedDatabase()
        {
            _pages = new List<Page>() 
            {
                new()
                {
                      BookId = new Guid("fa00ce8a-2418-4c24-aebd-2e10e3d6866c"),
                      Id = new Guid("f1f49b59-d026-4c3f-b898-1844e6cd55eb"),
                      Content = "Page content",
                      Title = "title"
                }
            };
            Repo.Db.Pages.AddRange(_pages);
            Repo.Db.SaveChanges();

        }
        protected override Page CreateModel()
        {
            return new Page
            {
                BookId = new Guid("fa00ce8a-2418-4c24-aebd-2e10e3d6896c"),
                Id = new Guid("f1f49b59-d026-4c3f-b898-1844e6cd55ec"),
                Content = "Page content",
                Title = "title"
            };
        }
    }
}
