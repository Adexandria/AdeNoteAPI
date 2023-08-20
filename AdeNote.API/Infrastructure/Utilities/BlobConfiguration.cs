namespace AdeNote.Infrastructure.Utilities
{
    /// <summary>
    /// Contains configuration for blob storage
    /// </summary>
    public class BlobConfiguration
    {
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
