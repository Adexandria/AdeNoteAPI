using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Downloads image
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>html</returns>
        string DownloadImage(string fileName, MimeType mimeType = MimeType.html);

        /// <summary>
        /// Downloads image
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>html</returns>
        Stream DownloadStream(string fileName, MimeType mimeType = MimeType.html);

        /// <summary>
        /// Uploads image
        /// </summary>
        /// <param name="fileName">Image name</param>
        /// <param name="file">Image</param>
        /// <returns>a url</returns>
        void UploadImage(string fileName, Stream file, MimeType mimeType = MimeType.png);
    }
}
