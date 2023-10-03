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
                .Get<BlobConfiguration>();
        }

        /// <summary>
        /// Uploads image
        /// </summary>
        /// <param name="fileName">Image name</param>
        /// <param name="file">Image</param>
        /// <returns>a url</returns>
        public async Task<string> UploadImage(string fileName, Stream file)
        {
            var blobUri = GenerateBlobUri(fileName);
            var storageCredentials = GenerateStorageCredentials();
            var blobClient = new BlobClient(blobUri, storageCredentials);
            await blobClient.UploadAsync(file,true);
            await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
            {
                ContentType = "image/png"
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
        private Uri GenerateBlobUri(string fileName)
        {
            return new Uri($"https://{ _blobConfig.AccountName}.blob.core.windows.net/{ _blobConfig.Container}/{fileName}.png");
        }

        /// <summary>
        /// Generates uri 
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>Uri</returns>
        private Uri GenerateBlobHTMLUri(string fileName)
        {
            return new Uri($"https://{ _blobConfig.AccountName}.blob.core.windows.net/{ _blobConfig.Container}/{fileName}.html");
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
        /// <returns>Html content</returns>
        public async Task<string> DownloadImage(string fileName)
        {
            using var ms = new MemoryStream();
            var blobUri = GenerateBlobHTMLUri(fileName);
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
