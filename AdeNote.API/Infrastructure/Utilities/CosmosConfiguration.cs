using ChattyPie.Utilities;

namespace AdeNote.Infrastructure.Utilities
{
    public class CosmosConfiguration : ICosmosConfiguration
    {
        public string Endpoint { get; set; }
        public string TokenCredential { get; set; }
        public string DatabaseId { get; set; }
    }
}
