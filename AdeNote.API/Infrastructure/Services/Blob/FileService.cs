using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services.Blob
{
    public class FileService : IFileService
    {
        public string UploadImage(string fileName, Stream file, MimeType mimeType = MimeType.png)
        {
            try
            {
                using var fileStream = new FileStream($"Template/{fileName}.{mimeType}", FileMode.OpenOrCreate);

                var blob = ConvertToBytes(file);

                fileStream.Write(blob, 0, blob.Length);

                return "Success";
            }
            catch (Exception)
            {
                return $"Unable to export file to {mimeType} format";
            }
            
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

            var memoryStream = new MemoryStream();

            fileStream.CopyTo(memoryStream);

            memoryStream.Position = 0;

            return memoryStream;
        }


        private byte[] ConvertToBytes(Stream stream)
        {
            byte[] bytes;

            stream.Position = 0;

            using (var binaryReader = new BinaryReader(stream))
            {
                bytes = binaryReader.ReadBytes((int)stream.Length);
            }
            return bytes;
        }
    }
}
