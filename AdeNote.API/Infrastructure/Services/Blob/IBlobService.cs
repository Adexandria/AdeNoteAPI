using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services.Blob
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
        Task<string> UploadImage(string fileName, Stream file,CancellationToken cancellationToken = default, MimeType mimeType = MimeType.png);

        /// <summary>
        /// Deletes image
        /// </summary>
        /// <param name="fileUrl">File url</param>
        /// <returns>True if deleted</returns>
        Task<bool> DeleteImage(string fileUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Downloads image
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>html</returns>
        Task<string> DownloadImage(string fileName, CancellationToken cancellationToken = default, MimeType mimeType = MimeType.html);

        /// <summary>
        /// Downloads image
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>html</returns>
        Task<Stream> DownloadStream(string fileName, CancellationToken cancellationToken= default, MimeType mimeType = MimeType.html);
    }
}
