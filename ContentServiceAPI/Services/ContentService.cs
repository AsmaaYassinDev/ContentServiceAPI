using Azure.Storage.Blobs;
using ContentServiceAPI.Models;
using Microsoft.Azure.Cosmos;

namespace ContentServiceAPI.Services
{
    public class ContentService : IContentService
    {
        private readonly Container _container;
        private readonly BlobContainerClient _blobContainerClient;

        public ContentService(CosmosClient cosmosClient, IConfiguration config)
        {
            _container = cosmosClient
                .GetDatabase(config["CosmosDb:DatabaseName"])
                .GetContainer(config["CosmosDb:ContainerName"]);

            var blobServiceClient = new BlobServiceClient(config["AzureBlob:ConnectionString"]);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(config["AzureBlob:ContainerName"]);
            _blobContainerClient.CreateIfNotExists();  // Ensure the container exists
        }

        // Add content (photo/video)
        public async Task<ContentItem> AddContentAsync(ContentItemRequest request)
        {
            if (request.MediaFile == null || request.MediaFile.Length == 0)
            {
                throw new ArgumentException("Media file is missing.");
            }

            try
            {
                // Create a unique name for the blob
                string blobName = Guid.NewGuid() + Path.GetExtension(request.MediaFile.FileName);
                var blobClient = _blobContainerClient.GetBlobClient(blobName);

                // Upload the media file to blob storage
                using (var stream = request.MediaFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true); // Overwrite if exists
                }

                // Create a new ContentItem with metadata
                var item = new ContentItem
                {
                    id = Guid.NewGuid().ToString(),
                    userId = request.userId,
                    Title = request.Title,
                    Caption = request.Caption,
                    Location = request.Location,
                    Tags = request.Tags,
                    UploadDate = DateTime.UtcNow,
                    MediaUrl = blobClient.Uri.ToString() // Store the URL of the blob
                };

                // Save metadata to Cosmos DB
                await _container.CreateItemAsync(item, new PartitionKey(item.userId));
                return item;
            }
            catch (Exception ex)
            {
                // Log and handle the error appropriately
                throw new ApplicationException("Error uploading content.", ex);
            }
        }

        // Get photo by its ID
        public async Task<ContentItem> GetPhotoByIdAsync(string id)
        {
            try
            {
                var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{id}'";
                var queryResultSetIterator = _container.GetItemQueryIterator<ContentItem>(sqlQueryText);
                var response = await queryResultSetIterator.ReadNextAsync();
                return response.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Log the error if necessary
                throw new ApplicationException("Error retrieving photo by ID.", ex);
            }
        }

        // Get photos by userId
        public async Task<IEnumerable<ContentItem>> GetPhotosByUserAsync(string userId)
        {
            try
            {
                var sqlQueryText = $"SELECT * FROM c WHERE c.userId = '{userId}'";
                var queryResultSetIterator = _container.GetItemQueryIterator<ContentItem>(sqlQueryText);
                var response = await queryResultSetIterator.ReadNextAsync();
                return response;
            }
            catch (Exception ex)
            {
                // Log the error if necessary
                throw new ApplicationException("Error retrieving photos by user.", ex);
            }
        }
    }
}