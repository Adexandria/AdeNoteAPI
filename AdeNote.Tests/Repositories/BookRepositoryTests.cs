using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using Moq;

namespace AdeNote.Tests.Repositories
{
    [TestFixture]
    public class BookRepositoryTests : RepoTestHelpher<BookRepository,Book>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            var HasSeeded = Repo.Db.Books.Any();
            if (!HasSeeded)
            {
                SeedDatabase();
            }
        }


        [Test]
        public async Task ShouldAddBookSuccessfully()
        {
            //Arrange
            AssumeSaveChangesSuccessfully();
            //Act
            var response = await Repo.Add(obj);

            //Assert
            Assert.That(response, Is.EqualTo(true));

        }

        [Test]
        public void ShouldGetAllBooksSuccessfully()
        {
            //Act
            var response = Repo.GetAll(new Guid("6b9da688-9591-4e3c-bf8c-732dd3080866"));

            //Assert
            Assert.That(response.Count, Is.EqualTo(1));

        }

        [Test]
        public async Task ShouldGetBookSuccessfully()
        {
            //Act
            var response = await Repo.GetAsync(new Guid("a557773f-f8c3-47af-a16f-d144dfc0c64b"), new Guid("6b9da688-9591-4e3c-bf8c-732dd3080866"));

            //Assert
            Assert.That(response, Is.Null);
        }

        [Test]
        public async Task ShouldRemoveBookSuccessfully()
        {
            //Arrange
            AssumeSaveChangesSuccessfully();

            //Act
            var response = await Repo.Remove(obj);

            //Assert
            Assert.That(response, Is.EqualTo(true));
        }

        [Test]
        public async Task ShouldUpdateBookSuccessfully()
        {
            //Arrange
            AssumeSaveChangesSuccessfully();
             
            //Act
            var response = await Repo.Update(books.FirstOrDefault());

            //Assert
            Assert.That(response, Is.EqualTo(true));
        }

        protected override Book CreateModel()
        {
            return new Book()
            {
              Description ="yes",
              Title = "yes",
              UserId = new Guid("6b9da688-9591-4e3c-bf8c-732dd3080866")
            };

        }

        private List<Book> books;

        private void SeedDatabase()
        {
            books = new List<Book>()
            {
                new()
                {
                    Id = new Guid("a557773f-f8c3-47af-a16f-d144dfc0c64b"),
                    Title = "title 1",
                    Description ="Hello",
                    UserId = new Guid("6b9da688-9591-4e3c-bf8c-732dd3080866")
                }
            };
            Repo.Db.Books.AddRange(books);
            Repo.Db.SaveChanges();
        }
    }
}
