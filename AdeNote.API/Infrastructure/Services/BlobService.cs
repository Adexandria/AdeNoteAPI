using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Utilities;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;

namespace AdeNote.Infrastructure.Services
{
    /// <summary>
    /// Handles the implementation of cloud storage
    /// </summary>
    public class BlobService : IBlobService
    {
        private BlobConfiguration _blobConfig;

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="_configuration">Reads the key/value pair from appsettings</param>
        public BlobService(IConfiguration _configuration)
        {
            _blobConfig = _configuration.GetSection("AzureStorageSecret")
                .Get<BlobConfiguration>() ?? new BlobConfiguration(
                    _configuration.GetValue<string>("AzureStorageSecret__AccountKey"),
                    _configuration.GetValue<string>("AzureStorageSecret__AccountName"),
                    _configuration.GetValue<string>("AzureStorageSecret__Container"));
        }

        /// <summary>
        /// Uploads image
        /// </summary>
        /// <param name="fileName">Image name</param>
        /// <param name="file">Image</param>
        /// <param name="mimeType">Mime type of file</param>
        /// <returns>a url</returns>
        public async Task<string> UploadImage(string fileName, Stream file, MimeType mimeType)
        {
            var blobUri = GenerateBlobUri(fileName,mimeType);
            var storageCredentials = GenerateStorageCredentials();
            var blobClient = new BlobClient(blobUri, storageCredentials);
            await blobClient.UploadAsync(file,true);
            await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
            {
                ContentType = mimeType.GetDescription()
            });
            return blobUri.AbsoluteUri;
        }

        /// <summary>
        /// Creates a shared key for the account
        /// </summary>
        /// <returns>StorageSharedKeyCredential</returns>
        private StorageSharedKeyCredential GenerateStorageCredentials()
        {
            return new StorageSharedKeyCredential(_blobConfig.AccountName,
                _blobConfig.AccountKey);
        }

        /// <summary>
        /// Generates uri 
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>Uri</returns>
        private Uri GenerateBlobUri(string fileName,MimeType mimeType)
        {
            return new Uri($"https://{ _blobConfig.AccountName}.blob.core.windows.net/{ _blobConfig.Container}/{fileName}.{mimeType}");
        }


        /// <summary>
        /// Deletes Image
        /// </summary>
        /// <param name="fileUrl">file url</param>
        /// <returns>True if deleted</returns>
        public async Task<bool> DeleteImage(string fileUrl)
        {
            var storageCredentials = GenerateStorageCredentials();
            var blobClient = new BlobClient(new Uri(fileUrl), storageCredentials);
            return await blobClient.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Downloads image
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="mimeType">mime type of the file</param>
        /// <returns>Html content</returns>
        public async Task<string> DownloadImage(string fileName, MimeType mimeType)
        {
            using var ms = new MemoryStream();
            var blobUri = GenerateBlobUri(fileName,mimeType);
            var storageCredentials = GenerateStorageCredentials();
            var blobClient = new BlobClient(blobUri, storageCredentials);

            var response = await blobClient.DownloadToAsync(ms);
            ms.Position = 0;
            using var reader = new StreamReader(ms, Encoding.UTF8);
            var data = reader.ReadToEnd();

            return data;
        }
    }
}
