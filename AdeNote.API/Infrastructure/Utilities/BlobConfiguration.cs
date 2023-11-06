namespace AdeNote.Infrastructure.Utilities
{
    /// <summary>
    /// Contains configuration for blob storage
    /// </summary>
    public class BlobConfiguration
    {
        public BlobConfiguration()
        {

        }
        public BlobConfiguration(string accountKey, string accountName, string container)
        {
            AccountKey = accountKey;
            AccountName = accountName;
            Container = container;
        }
        /// <summary>
        /// Account key
        /// </summary>
        public string AccountKey { get; set; }  


        /// <summary>
        /// Account Name
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Container
        /// </summary>
        public string Container { get; set; }
    }
}
