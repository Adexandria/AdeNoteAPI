using AdeCache.Services;
using Automapify.Services;
using Automappify.Services;
using Moq;


namespace AdeNote.Tests
{
    public class ServiceTestHelpher<TService,TRepo,TModel,TUpdateModel> where TService : class 
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
           CacheService = new Mock<ICacheService>();
        }

        protected virtual TModel CreateModel()
        {
            return new Mock<TModel>().Object;
        }

        protected virtual TUpdateModel MapModel(MapifyConfiguration config = null)
        {
            if(config == null)
                return obj.Map<TModel,TUpdateModel>();

            return obj.Map<TModel, TUpdateModel>();
        }

        protected TModel obj { get; set; }
        protected TUpdateModel updateObj { get; set; }
        protected TService Service { get; set; }
        protected Mock<ICacheService> CacheService { get; set; }
        protected Mock<TRepo> Repo { get; set; }
    }
}
