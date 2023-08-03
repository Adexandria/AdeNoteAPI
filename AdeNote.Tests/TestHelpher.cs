using Mapster;
using Moq;


namespace AdeNote.Tests
{
    public class TestHelpher<TService,TRepo,TModel,TUpdateModel> where TService : class 
        where TRepo : class
        where TModel : class
        where TUpdateModel : class
    {
        [SetUp]
        protected virtual void Setup()
        {
           Service = new Mock<TService>().Object;
           Repo = new Mock<TRepo>();
           obj = CreateModel();
           updateObj = MapModel();
        }

        protected virtual TModel CreateModel()
        {
            return new Mock<TModel>().Object;
        }

        protected virtual TUpdateModel MapModel(TypeAdapterConfig config = null)
        {
            if(config == null)
                return obj.Adapt<TUpdateModel>();

            return obj.Adapt<TUpdateModel>(config);
        }

        protected TModel obj { get; set; }
        protected TUpdateModel updateObj { get; set; }
        protected TService Service { get; set; }
        protected Mock<TRepo> Repo { get; set; }
    }
}
