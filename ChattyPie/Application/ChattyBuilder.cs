using ChattyPie.Db;
using ChattyPie.Infrastructure.Interfaces;
using ChattyPie.Infrastructure.Repositories;
using ChattyPie.Utilities;
using Microsoft.Extensions.DependencyInjection;


namespace ChattyPie.Application
{
    public static class ChattyBuilder
    {
        public static void RegisterChattyPie(this IServiceCollection serviceCollection,
            ICosmosConfiguration cosmosConfiguration)
        {
            serviceCollection.AddSingleton((_) => 
            new CosmosFactory(cosmosConfiguration).InitialiseDatabase());

            serviceCollection.AddScoped<IThreadRepository, ThreadRepository>();

            serviceCollection.AddScoped<ISubThreadRepository, SubThreadRepository>();

            serviceCollection.AddTransient<IChatApplication, ChatApplication>();

            serviceCollection.AddScoped<ISearchRepository,SearchRepository>();

            serviceCollection.AddScoped<ThreadQuery>();
        }

    }
}
