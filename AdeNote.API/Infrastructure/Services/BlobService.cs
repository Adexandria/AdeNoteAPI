using AdeNote.Infrastructure.Utilities;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AdeNote.Infrastructure.Services
{
    public class BlobService : IBlobService
    {
        private BlobConfiguration _blobConfig;

        public BlobService(IConfiguration _configuration)
        {
            _blobConfig = _configuration.GetSection("AzureStorageSecret")
                .Get<BlobConfiguration>();
        }

        public async Task<string> UploadFile(string fileName, Stream file)
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


        private StorageSharedKeyCredential GenerateStorageCredentials()
        {
            return new StorageSharedKeyCredential(_blobConfig.AccountName,
                _blobConfig.AccountKey);
        }

        private Uri GenerateBlobUri(string fileName)
        {
            return new Uri($"https://{ _blobConfig.AccountName}.blob.core.windows.net/{ _blobConfig.Container}/{fileName}.png");
        }

    }
}
