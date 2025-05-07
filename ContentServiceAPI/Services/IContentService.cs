using ContentServiceAPI.Models;

namespace ContentServiceAPI.Services
{
    public interface IContentService
    {
        // Method to add content (photo/video)
        Task<ContentItem> AddContentAsync(ContentItemRequest request);

        // Method to get content by its ID
        Task<ContentItem> GetPhotoByIdAsync(string id);

        // Method to get all content by userId
        Task<IEnumerable<ContentItem>> GetPhotosByUserAsync(string userId);
    }
}
