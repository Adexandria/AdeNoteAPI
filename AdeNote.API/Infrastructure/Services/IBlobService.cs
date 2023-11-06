using AdeNote.Infrastructure.Utilities;

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
        Task<string> UploadImage(string fileName, Stream file,MimeType mimeType = MimeType.png);

        /// <summary>
        /// Deletes image
        /// </summary>
        /// <param name="fileUrl">File url</param>
        /// <returns>True if deleted</returns>
        Task<bool> DeleteImage(string fileUrl);

        /// <summary>
        /// Downloads image
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>html</returns>
        Task<string> DownloadImage(string fileName, MimeType mimeType = MimeType.html);
    }
}
