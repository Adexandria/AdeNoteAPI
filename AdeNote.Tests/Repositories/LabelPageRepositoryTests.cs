using AdeNote.Infrastructure.Repository;
using AdeNote.Models;

namespace AdeNote.Tests.Repositories
{
    public class LabelPageRepositoryTests: RepoTestHelpher<LabelPageRepository,LabelPage>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            var hasSeeded = Repo.Db.LabelPage.Any();
            if (!hasSeeded)
                SeedDatabase();
        }
        [Test]
        public async Task ShouldAddLabelToPageSuccessfully()
        {
            //Arrange
            AssumeSaveChangesSuccessfully();
            //Act
            var response = await Repo.AddLabelToPage(obj.PageId,obj.LabelId);

            //Assert
            Assert.That(response, Is.EqualTo(true));

        }

        [Test]
        public async Task ShouldGetAllBooksSuccessfully()
        {
            //Act
            var response = await Repo.GetLabels(labelPages.Select(s => s.PageId).FirstOrDefault());

            //Assert
            Assert.That(response.Count, Is.EqualTo(1));

        }

        [Test]
        public async Task ShouldGetLabelsSuccessfully()
        {
            //Act
            var response = await Repo.GetLabel(labelPages.Select(s=>s.PageId).FirstOrDefault(), labelPages.Select(s => s.LabelId).FirstOrDefault());

            //Assert
            Assert.That(response, Is.TypeOf<LabelPage>());
        }

        [Test]
        public async Task ShouldRemoveLabelsFromPageSuccessfully()
        {
            //Arrange
            AssumeSaveChangesSuccessfully();

            //Act
            var response = await Repo.DeleteLabelsFromPage(labelPages);

            //Assert
            Assert.That(response, Is.EqualTo(true));
        }

        [Test]
        public async Task ShouldUpdateLabelFromPageSuccessfully()
        {
            //Arrange
            AssumeSaveChangesSuccessfully();

            //Act
            var response = await Repo.DeleteLabelFromPage(obj);

            //Assert
            Assert.That(response, Is.EqualTo(true));
        }


        protected override LabelPage CreateModel()
        {
            return new LabelPage() 
            {
                LabelId = new Guid("fa00ce8a-2418-4c24-aebd-2e10e3d6869c"),
                PageId = new Guid("f1f49b59-d026-4c3f-b898-1844e6cd57eb")
            };

        }
        private List<LabelPage> labelPages;
        private void SeedDatabase()
        {
            labelPages = new List<LabelPage>()
            {
                new()
                {
                    LabelId = new Guid("fa00ce8a-2418-4c24-aebd-2e10e3d6866c"),
                    PageId = new Guid("f1f49b59-d026-4c3f-b898-1844e6cd55eb")
                }
            };
            Repo.Db.LabelPage.AddRange(labelPages);
            Repo.Db.SaveChanges();
        }
    }
}
