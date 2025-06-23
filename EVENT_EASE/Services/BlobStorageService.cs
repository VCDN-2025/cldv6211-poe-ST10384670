using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Threading.Tasks;

namespace EVENT_EASE.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private BlobContainerClient _containerClient;

        // ✅ This is the constructor the Program.cs is calling
        public BlobStorageService(string connectionString, string containerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = containerName;
        }

        // ✅ Create or get the container
        public async Task<BlobContainerClient> GetContainerAsync()
        {
            if (_containerClient != null)
                return _containerClient;

            _containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            return _containerClient;
        }

        // ✅ Get a specific blob client
        public BlobClient GetBlobClient(string fileName)
        {
            if (_containerClient == null)
                throw new InvalidOperationException("Container has not been initialized. Call GetContainerAsync() first.");

            return _containerClient.GetBlobClient(fileName);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty");

            // ✅ Ensure the container is initialized
            var container = await GetContainerAsync();

            // ✅ Generate a unique file name
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            // ✅ Get a reference to the blob
            var blobClient = container.GetBlobClient(fileName);

            // ✅ Upload the file
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString();
        }

    }
}
