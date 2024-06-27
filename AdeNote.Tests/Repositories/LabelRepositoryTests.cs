using AdeNote.Infrastructure.Repository;
using AdeNote.Models;


namespace AdeNote.Tests.Repositories
{
    public class LabelRepositoryTests: RepoTestHelpher<LabelRepository,Label>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            var HasSeeded = Repo.Db.Labels.Any();
            if (!HasSeeded)
                SeedDatabase();
        }

        [Test]
        public async Task ShouldAddLabelSuccessfully()
        {
            AssumeSaveChangesSuccessfully();
            //Act
            var response = await Repo.Add(obj);

            //Assert
            Assert.That(response, Is.EqualTo(true));
        }

        [Test]
        public void ShouldGetAllLabelsSuccessfully()
        {
            //Act
            var response = Repo.GetAll();

            //Assert
            Assert.That(response.Count, Is.EqualTo(1));

        }

        [Test]
        public async Task ShouldGetLabelSuccessfully()
        {
            //Act
            var response = await Repo.GetNoTrackingAsync(new Guid("c80fbb29-6d17-40a7-bb1a-c91770976848"));

            //Assert
            Assert.That(response, Is.TypeOf<Label>());
        }

        [Test]
        public async Task ShouldGetLabelByNameSuccessfully()
        {
            //Act
            var response = await Repo.GetByNameAsync("First label");

            //Assert
            Assert.That(response, Is.TypeOf<Label>());
        }

        [Test]
        public async Task ShouldRemoveLabelSuccessfully()
        {
            //Arrange
            AssumeSaveChangesSuccessfully();

            //Act
            var response = await Repo.Remove(obj);

            //Assert
            Assert.That(response, Is.EqualTo(true));
        }

        [Test]
        public async Task ShouldUpdateLableSuccessfully()
        {
            //Arrange
            AssumeSaveChangesSuccessfully();

            //Act
            var response = await Repo.Update(labels.FirstOrDefault());

            //Assert
            Assert.That(response, Is.EqualTo(true));
        }


        protected override Label CreateModel()
        {
            return new Label
            { 
                Id = new Guid("5160a51b-e968-4379-bf28-5b84965a11ff"),
                Title = "testing label 2"
            };

        }
        private List<Label> labels;

        private void SeedDatabase()
        {
            labels = new List<Label>()
            {
                new()
                {
                    Title ="First label",
                    Id = new Guid("c80fbb29-6d17-40a7-bb1a-c91770976848")
                }
            };
            Repo.Db.Labels.AddRange(labels);
            Repo.Db.SaveChanges();
        }
    }
}
