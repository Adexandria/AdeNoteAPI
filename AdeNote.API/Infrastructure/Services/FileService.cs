using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public class FileService : IFileService
    {
        public async void UploadImage(string fileName, Stream file, MimeType mimeType = MimeType.png)
        {
            try
            {
                using var fileStream = new FileStream($"Template/{fileName}.{mimeType}", FileMode.OpenOrCreate);

                using var memoryStream = new MemoryStream();

                await file.CopyToAsync(memoryStream);

                var blob = memoryStream.ToArray();

                fileStream.Write(blob, 0, blob.Length);
            }
            catch (Exception)
            {
                return;
            }
            
        }

        public string DownloadImage(string fileName, MimeType mimeType = MimeType.html)
        {
            try
            {
                using var fileStream = new FileStream($"Template/{fileName}.{mimeType}", FileMode.Open);

                using var streamReader = new StreamReader(fileStream);

                var blob = streamReader.ReadToEnd();

                return blob;

            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public Stream DownloadStream(string fileName, MimeType mimeType = MimeType.html)
        {
            try
            {
                using var fileStream = new FileStream($"Template/{fileName}.{mimeType}", FileMode.Open);

                var ms = new MemoryStream();

                fileStream.CopyTo(ms);

                ms.Position = 0;

                return ms;
            }
            catch (Exception)
            {
                return default;
            }
        }

       
    }
}
