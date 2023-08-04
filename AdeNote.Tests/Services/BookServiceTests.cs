using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Moq;

namespace AdeNote.Tests.Services
{
    [TestFixture]
    public class BookServiceTests : ServiceTestHelpher<BookService,IBookRepository,BookCreateDTO,BookUpdateDTO>
    {

        protected override void Setup()
        {
            base.Setup();
            Service.bookRepository = Repo.Object;
        }

        [Test]
        public async Task ShouldAddBookSuccessfully()
        {
            //Arrange
            Repo.Setup(s => s.Add(It.IsAny<Book>())).ReturnsAsync(true);

            //Act
            
            var response = await Service.Add(Guid.NewGuid(),obj);

            //Assert
            Assert.That(response.IsSuccessful, Is.EqualTo(true));

        }

        [Test]
        public async Task ShouldFailToAddBook()
        {
            //Arrange
            Repo.Setup(s => s.Add(It.IsAny<Book>())).ReturnsAsync(false);

            //Act

            var response = await Service.Add(Guid.NewGuid(), obj);

            //Assert
            Assert.That(response.IsSuccessful, Is.EqualTo(false));
        }

        [Test]
        public async Task ShouldGetAllBooksSuccessfully()
        {
            //Arrange
            var book1 = new Book("Test 1", "Description1");
            var book2 = new Book("Test 2", "Description2");

            Repo.Setup(s => s.GetAll(It.IsAny<Guid>())).Returns(new[] { book1,book2}.AsQueryable());

            //Act
            var response = await Service.GetAll(Guid.NewGuid());

            //Assert
            Assert.That(response.Data.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task ShouldGetBookByIdSuccessfully()
        {
            //Arrange
            var book1 = new Book("Test 1", "Description1");

            Repo.Setup(s=>s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(book1);

            //Act
            var response = await Service.GetById(Guid.NewGuid(),Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.EqualTo(true));
        }

        [Test]
        public async Task ShouldFailToGetBookByIdIfIdIsEmpty()
        {
            //Act
            var response = await Service.GetById(new Guid(), Guid.NewGuid());


            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.EqualTo(false));
                Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
            });
        }

        [Test]
        public async Task ShouldFailToGetBookByIdIfBookDoesNotExist()
        {
            //Arrange
            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.GetById(Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.EqualTo(false));
                Assert.That(response.Errors.Single(), Is.EqualTo("Book does not exist"));
            });
        }

        [Test]
        public async Task ShouldRemoveBookSuccessfully()
        {
            //Arrange
            var book1 = new Book("Test 1", "Description1");

            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(book1);
            Repo.Setup(s => s.Remove(It.IsAny<Book>())).ReturnsAsync(true);

            //Act
            var response = await Service.Remove(Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.EqualTo(true));
        }

        [Test]
        public async Task ShouldFailToRemoveBookIfIdIsEmpty()
        {
            //Act
            var response = await Service.Remove(new Guid(), Guid.NewGuid());

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.EqualTo(false));
                Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
            });
        }

        [Test]
        public async Task ShouldFailToRemoveBookIfBookDoesNotExist()
        {
            //Arrange
            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.Remove(Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.EqualTo(false));
                Assert.That(response.Errors.Single(), Is.EqualTo("Book does not exist"));
            });
        }

        [Test]
        public async Task ShouldFailToRemoveBook()
        {
            //Arrange
            var book1 = new Book("Test 1", "Description1");

            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(book1);
            Repo.Setup(s => s.Remove(It.IsAny<Book>()));

            //Act
            var response = await Service.Remove(Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.EqualTo(false));
                Assert.That(response.Errors.Single(), Is.EqualTo("Failed to delete book"));
            });
        }

        [Test]
        public async Task ShouldUpdateBookSuccessfully()
        {
            //Arrange
            var book1 = new Book("Test 1", "Description1");

            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(book1);
            Repo.Setup(s => s.Update(It.IsAny<Book>())).ReturnsAsync(true);

            //Act
            var response = await Service.Update(Guid.NewGuid(), Guid.NewGuid(),
               updateObj);

            //Assert
            Assert.That(response.IsSuccessful, Is.EqualTo(true));
        }

        [Test]
        public async Task ShouldFailToUpdateBookIfIdIsEmpty()
        {
            //Act
            var response = await Service.Update(new Guid(), Guid.NewGuid(),
               updateObj);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.EqualTo(false));
                Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
            });
        }

        [Test]
        public async Task ShouldFailToUpdateBookIfBookDoesNotExist()
        {
            //Arrange
            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.Update(Guid.NewGuid(), Guid.NewGuid(), 
               updateObj);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.EqualTo(false));
                Assert.That(response.Errors.Single(), Is.EqualTo("Book does not exist"));
            });
        }

        [Test]
        public async Task ShouldFailToUpdateBook()
        {
            //Arrange
            var book1 = new Book("Test 1", "Description1");

            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(book1);
            Repo.Setup(s => s.Update(It.IsAny<Book>()));

            //Act
            var response = await Service.Update(Guid.NewGuid(), Guid.NewGuid(), 
                updateObj);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.EqualTo(false));
                Assert.That(response.Errors.Single(), Is.EqualTo("Failed to update book"));
            });
        }
        protected override BookCreateDTO CreateModel()
        {
            return new BookCreateDTO
            {
                Description = "Description",
                Title = "Unit tests"
            };
        }
    }
}
