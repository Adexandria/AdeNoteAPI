using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Moq;

namespace AdeNote.Tests.Services
{
    [TestFixture]
    public class LabelServiceTests : TestHelpher<LabelService,ILabelRepository,LabelCreateDTO,LabelUpdateDTO>
    {
        [SetUp]
        protected override void Setup()
        {
            base.Setup();
            Service.labelRepository = Repo.Object;
        }

        [Test]
        public async Task ShouldAddLabelSuccessfully()
        {
            //Arrange
            Repo.Setup(s => s.Add(It.IsAny<Label>())).ReturnsAsync(true);

            //Act
            var response = await Service.Add(obj);

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToAddLabel()
        {
            //Act
            var response = await Service.Add(obj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Failed to add label"));
        }

        [Test]
        public async Task ShouldGetAllLabelsSuccessfully()
        {
            //Arrange
            var label1 = new Label("label1");
            var label2 = new Label("label2");

            Repo.Setup(s=>s.GetAll()).Returns(new[] { label1, label2 }.AsQueryable());

            //Act
            var response = await Service.GetAll();

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
            Assert.That(response.Data.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task ShouldGetLabelByIdSuccessfully()
        {
            //Arrange
            Repo.Setup(s=>s.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new Label("label1"));

            //Act
            var response = await Service.GetById(Guid.NewGuid());


            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToGetLabelByIdIfIdIsEmpty()
        {
            //Act
            var response = await Service.GetById(new Guid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldFailToGetLabelIfLabelDoesNotExist()
        {
            //Arrange
            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>()));

            //Act
            var response = await Service.GetById(Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Label doesn't exist"));

        }

        [Test]
        public async Task ShouldFailToRemoveLabelIfIdDoesNotExist()
        {
            //Act
            var response = await Service.Remove(new Guid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldRemoveLabelSuccessfully()
        {
            //Arrange
            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new Label("label1"));
            Repo.Setup(s => s.Remove(It.IsAny<Label>())).ReturnsAsync(true);

            //Act
            var response = await Service.Remove(Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToRemoveLabelIfLabelDoesNotExist()
        {
            //Act
            var response = await Service.Remove(Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("label doesn't exist"));
        }

        [Test]
        public async Task ShouldFailToRemoveLabel()
        {
            //Arrange
            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new Label("label1"));
            Repo.Setup(s => s.Remove(It.IsAny<Label>())).ReturnsAsync(false);

            //Act
            var response = await Service.Remove(Guid.NewGuid());

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Failed to remove label"));
        }


        [Test]
        public async Task ShouldFailToUpdateLabelIfIdDoesNotExist()
        {
            //Act
            var response = await Service.Update(new Guid(),updateObj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Invalid id"));
        }

        [Test]
        public async Task ShouldUpdateLabelSuccessfully()
        {
            //Arrange
            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new Label("label1"));
            Repo.Setup(s => s.Update(It.IsAny<Label>())).ReturnsAsync(true);

            //Act
            var response = await Service.Update(Guid.NewGuid(), updateObj);

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
        }

        [Test]
        public async Task ShouldFailToUpdateLabelIfLabelDoesNotExist()
        {
            //Act
            var response = await Service.Update(Guid.NewGuid(), updateObj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("label doesn't exist"));
        }

        [Test]
        public async Task ShouldFailToUpdateLabel()
        {
            //Arrange
            Repo.Setup(s => s.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new Label("label1"));
            Repo.Setup(s => s.Update(It.IsAny<Label>())).ReturnsAsync(false);

            //Act
            var response = await Service.Update(Guid.NewGuid(), updateObj);

            //Assert
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.Errors.Single(), Is.EqualTo("Failed to update label"));
        }
        protected override LabelCreateDTO CreateModel()
        {
            return new LabelCreateDTO
            {
                Title = "test title"
            };
        }
    }
}
