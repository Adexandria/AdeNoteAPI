using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Moq;

namespace AdeNote.Tests.Services
{
    [TestFixture]
    public class PageServiceTests :TestHelpher<PageService,IPageRepository,PageCreateDTO,PageUpdateDTO>
    {
        [SetUp]
        protected override void Setup()
        {
            base.Setup();
            bookRepo = new Mock<IBookRepository>();
            labelPageRepo = new Mock<ILabelPageRepository>();
            labelRepo = new Mock<ILabelRepository>();
            Service.pageRepository = Repo.Object;
            Service.bookRepository = bookRepo.Object;
            Service.labelRepository = labelRepo.Object;
            Service.labelPageRepository = labelPageRepo.Object;
        }

        [Test]
        public async Task ShouldAddPageSuccessfully()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.Add(It.IsAny<Page>())).ReturnsAsync(true);

            //Act
            var response = await Service.Add(Guid.NewGuid(), Guid.NewGuid(), obj);

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToAddIfIdDoesNotExist()
        {
            //Act
            var response = await Service.Add(new Guid(), Guid.NewGuid(), obj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldFailToAddPageIfBookDoesNotExist()
        {
            //Act
            var response = await Service.Add(Guid.NewGuid(), Guid.NewGuid(), obj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Book doesn't exist"));
        }

        [Test]
        public async Task ShouldFailToAddPage()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(),It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.Add(It.IsAny<Page>())).ReturnsAsync(false);

            //Act
            var response = await Service.Add(Guid.NewGuid(), Guid.NewGuid(), obj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Failed to add page"));
        }

        [Test]
        public async Task ShouldFailToGetPagesIfIdDoesNotExist()
        {
            //Act
            var response = await Service.GetAll(new Guid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldGetAllPagesInBookSuccessfully()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            Repo.Setup(s => s.GetBookPages(It.IsAny<Guid>())).Returns(new[] { page }.AsQueryable());

            //Act
            var response = await Service.GetAll(Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.True);

        }

        [Test]
        public async Task ShouldFailToGetBookIfIdDoesNotExist()
        {
            //Act
            var response = await Service.GetById(new Guid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldGetPageById()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);

            //Act
            var response = await Service.GetById(Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToGetPageByIdIfPageDoesNotExist()
        {
            //Arrange
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.GetById(Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("page doesn't exist"));
        }

        [Test]
        public async Task ShouldFailToRemovePageIfIdDoesNotExist()
        {
            //Act
            var response = await Service.Remove(new Guid(), Guid.NewGuid(),Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldRemovePageSuccessfully()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            Repo.Setup(s => s.Remove(It.IsAny<Page>())).ReturnsAsync(true);

            //Act
            var response = await Service.Remove(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToRemovePageIfBookDoesNotExist()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.Remove(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Book doesn't exist"));
        }

        [Test]
        public async Task ShouldFailToRemovePageIfPageDoesNotExist()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.Remove(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("page doesn't exist"));

        }

        [Test]
        public async Task ShouldFailToRemovePage()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            Repo.Setup(s => s.Remove(It.IsAny<Page>())).ReturnsAsync(false);

            //Act
            var response = await Service.Remove(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Failed to delete page"));
        }

        [Test]
        public async Task ShouldFailToUpdatePageIfIdDoesNotExist()
        {
            //Act
            var response = await Service.Update(new Guid(), Guid.NewGuid(), Guid.NewGuid(),updateObj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldUpdatePageSuccessfully()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            Repo.Setup(s => s.Update(It.IsAny<Page>())).ReturnsAsync(true);

            //Act
            var response = await Service.Update(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),updateObj);

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToUpdatePageIfBookDoesNotExist()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.Update(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),updateObj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Book doesn't exist"));
        }

        [Test]
        public async Task ShouldFailToUpdatePageIfPageDoesNotExist()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.Update(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), updateObj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("page doesn't exist"));

        }

        [Test]
        public async Task ShouldFailToUpdatePage()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            Repo.Setup(s => s.Update(It.IsAny<Page>())).ReturnsAsync(false);

            //Act
            var response = await Service.Update(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), updateObj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Failed to update page"));
        }

        [Test]
        public async Task ShouldFailToRemoveBookIfIdDoesNotExist()
        {
            //Act
            var response = await Service.Remove(new Guid(), Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldAddLabelsPageSuccessfully()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            labelPageRepo.Setup(s => s.AddLabelToPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);
            labelRepo.Setup(s => s.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(new Label("testing label"));

            //Act
            var response = await Service.AddLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),new List<string>() { "Hello testing 1"});

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToAddLabelsToPageIfBookDoesNotExist()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.AddLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new List<string>() { "Hello testing 1" });

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Book doesn't exist"));
        }

        [Test]
        public async Task ShouldFailToAddlabelsToPageIfPageDoesNotExist()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.AddLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new List<string>() { "Hello testing 1" });

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("page doesn't exist"));

        }

        [Test]
        public async Task ShouldFailToAddLabelsToPageIfLabelDoesNotExist()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            Repo.Setup(s => s.Remove(It.IsAny<Page>())).ReturnsAsync(false);
            labelPageRepo.Setup(s => s.AddLabelToPage(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.AddLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new List<string>() { "Hello testing 1" });

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Label doesn't exist"));
        }

        [Test]
        public async Task ShouldFailToAddLabelsToPageIfLabelHasBeenAdded()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            page.Labels.Add(new Label("testing label"));
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            Repo.Setup(s => s.Remove(It.IsAny<Page>())).ReturnsAsync(false);
            labelRepo.Setup(s => s.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(new Label("testing label"));
            
            //Act
            var response = await Service.AddLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new List<string>() { "Hello testing 1" });

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Label has been added"));
        }

        [Test]
        public async Task ShouldFailToAddLabelsToPage()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            Repo.Setup(s => s.Remove(It.IsAny<Page>())).ReturnsAsync(false);
            labelRepo.Setup(s => s.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(new Label("testing label"));
            //Act
            var response = await Service.AddLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new List<string>() { "Hello testing 1" });

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Failed to add label"));
        }

        [Test]
        public async Task ShouldFailToRemoveAllPagelabelsIfIdDoesNotExist()
        {
            //Act
            var response = await Service.RemoveAllPageLabels(new Guid(), Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldRemoveAllPageLabelsSuccessfully()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            IList<LabelPage> labels = new List<LabelPage>() { new LabelPage() };
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            labelPageRepo.Setup(s => s.GetLabels(It.IsAny<Guid>())).ReturnsAsync(labels);
            labelPageRepo.Setup(s=>s.DeleteLabelsFromPage(It.IsAny<IList<LabelPage>>())).ReturnsAsync(true);

            //Act
            var response = await Service.RemoveAllPageLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToRemoveAllPageLabelsIfBookDoesNotExist()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.RemoveAllPageLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Book doesn't exist"));
        }

        [Test]
        public async Task ShouldFailToRemoveAllPageLabelsIfPageDoesNotExist()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.RemoveAllPageLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("page doesn't exist"));

        }

        [Test]
        public async Task ShouldFailToRemoveAllPageLabelsIfLabelsDoesNotExist()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            Repo.Setup(s => s.Remove(It.IsAny<Page>())).ReturnsAsync(false);
            labelPageRepo.Setup(s => s.GetLabels(It.IsAny<Guid>())).ReturnsAsync(new List<LabelPage>());

            //Act
            var response = await Service.RemoveAllPageLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Labels doesn't exist in this page"));
        }

        [Test]
        public async Task ShouldFailToRemoveAllPageLabels()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            IList<LabelPage> labels = new List<LabelPage>() { new LabelPage() };
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            Repo.Setup(s => s.Remove(It.IsAny<Page>())).ReturnsAsync(false);
            labelPageRepo.Setup(s => s.GetLabels(It.IsAny<Guid>())).ReturnsAsync(labels);

            //Act
            var response = await Service.RemoveAllPageLabels(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Failed to delete labels"));
        }

        [Test]
        public async Task ShouldFailToRemovePagelabelIfIdDoesNotExist()
        {
            //Act
            var response = await Service.RemovePageLabel(new Guid(), Guid.NewGuid(), Guid.NewGuid(),"Hello");

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldRemovePageLabelSuccessfully()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            var currentlabel = new Label("Hello");
            currentlabel.Id = Guid.NewGuid();
            page.Labels.Add(currentlabel);
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            labelPageRepo.Setup(s => s.GetLabel(It.IsAny<Guid>(),It.IsAny<Guid>())).ReturnsAsync(new LabelPage());
            labelPageRepo.Setup(s => s.DeleteLabelFromPage(It.IsAny<LabelPage>())).ReturnsAsync(true);

            //Act
            var response = await Service.RemovePageLabel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Hello");

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToRemovePageLabelIfBookDoesNotExist()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.RemovePageLabel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Hello");

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Book doesn't exist"));
        }

        [Test]
        public async Task ShouldFailToRemovePageLabelIfPageDoesNotExist()
        {
            //Arrange
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>()));

            //Act
            var response = await Service.RemovePageLabel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Hello");

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("page doesn't exist"));

        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("aa38dbf1dd7445fdb8b14ccbabd9fdcd")]
        public async Task ShouldFailToRemovePageLabelIfLabelDoesNotExist(string labelId)
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            var currentlabel = new Label("Hello");
            currentlabel.Id = new Guid(labelId);
            page.Labels.Add(currentlabel);
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            Repo.Setup(s => s.Remove(It.IsAny<Page>())).ReturnsAsync(false);

            //Act
            var response = await Service.RemovePageLabel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Hello");

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Label doesn't exist in this page"));
        }

        [Test]
        public async Task ShouldFailToRemoveAllPageLabel()
        {
            //Arrange
            var page = new Page("testing page");
            page.Book = new Book();
            var currentlabel = new Label("Hello");
            currentlabel.Id = Guid.NewGuid();
            page.Labels.Add(currentlabel);
            bookRepo.Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new Book());
            Repo.Setup(s => s.GetBookPage(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(page);
            labelPageRepo.Setup(s => s.GetLabel(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new LabelPage());

            //Act
            var response = await Service.RemovePageLabel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Hello");

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Failed to delete label"));
        }
        protected override PageCreateDTO CreateModel()
        {
            return new PageCreateDTO() 
            {
                Content = "hello test",
                Title = "Title"
            };

        }
        private Mock<IBookRepository> bookRepo;
        private Mock<ILabelPageRepository> labelPageRepo;
        private Mock<ILabelRepository> labelRepo;
    }
}
