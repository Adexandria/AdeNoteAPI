using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services.Blob
{
    public class FileService : IFileService
    {
        public async void UploadImage(string fileName, Stream file, MimeType mimeType = MimeType.png)
        {
            using var fileStream = new FileStream($"Template/{fileName}.{mimeType}", FileMode.OpenOrCreate);

            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            var blob = memoryStream.ToArray();

            fileStream.Write(blob, 0, blob.Length);

        }

        public string DownloadImage(string fileName, MimeType mimeType = MimeType.html)
        {
            using var fileStream = new FileStream($"Template/{fileName}.{mimeType}", FileMode.Open);

            using var streamReader = new StreamReader(fileStream);

            var blob = streamReader.ReadToEnd();

            return blob;

        }

        public Stream DownloadStream(string fileName, MimeType mimeType = MimeType.html)
        {
            using var fileStream = new FileStream($"Template/{fileName}.{mimeType}", FileMode.Open);

            var ms = new MemoryStream();

            fileStream.CopyTo(ms);

            ms.Position = 0;

            return ms;
        }

    }
}
