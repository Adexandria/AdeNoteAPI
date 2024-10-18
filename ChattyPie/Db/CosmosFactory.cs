using ChattPie.Db;
using ChattyPie.Utilities;
using Microsoft.Azure.Cosmos;

namespace ChattyPie.Db
{
    public class CosmosFactory : ICosmosFactory
    {
        public CosmosFactory(ICosmosConfiguration _cosmosConfiguration)
        {
           cosmosConfiguration = _cosmosConfiguration;
        }
        public Database InitialiseDatabase()
        {
            var cosmosClient = new CosmosClient(cosmosConfiguration.Endpoint, cosmosConfiguration.TokenCredential);

            var databaseResponse = cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosConfiguration.DatabaseId).Result;

            return databaseResponse.Database;
        }

        private readonly ICosmosConfiguration cosmosConfiguration;
    }
}
