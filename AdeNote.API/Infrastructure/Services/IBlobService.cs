namespace AdeNote.Infrastructure.Services
{
    /// <summary>
    /// An interface that includes the behaviour of cloud storage
    /// </summary>
    public interface IBlobService
    {
        /// <summary>
        /// Uploads image
        /// </summary>
        /// <param name="fileName">Image name</param>
        /// <param name="file">Image</param>
        /// <returns>a url</returns>
        Task<string> UploadImage(string fileName, Stream file);
    }
}
