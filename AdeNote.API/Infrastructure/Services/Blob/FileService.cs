using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services.Blob
{
    public class FileService : IFileService
    {
        public FileService()
        {
            _basePath = Environment.CurrentDirectory;
        }
        public string UploadImage(string fileName, Stream file, MimeType mimeType = MimeType.png)
        {
            try
            {
                var filePath = Path.Combine(_basePath, $"Template/{fileName}.{mimeType}");

                using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate);

                var blob = ConvertToBytes(file);

                fileStream.Write(blob, 0, blob.Length);

                return filePath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string DownloadImage(string fileName, MimeType mimeType = MimeType.html)
        {
            var filePath = Path.Combine(_basePath, $"Template/{fileName}.{mimeType}");

            if(!File.Exists(filePath))
            {
                return string.Empty;
            }

            var fileStream = File.OpenRead(filePath);

            using var streamReader = new StreamReader(fileStream);

            var blob = streamReader.ReadToEnd();

            return blob;
        }

        public Stream DownloadStream(string fileName, MimeType mimeType = MimeType.html)
        {
            var filePath = Path.Combine(_basePath, $"Template/{fileName}.{mimeType}");

            if (!File.Exists(filePath))
            {
                return default;
            }

            var fileStream = File.OpenRead(filePath);

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

        private readonly string _basePath;
    }
}
