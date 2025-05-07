using ContentServiceAPI.Models;
using ContentServiceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContentServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }

        // Upload a photo or video
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] ContentItemRequest request)
        {
            // Validate request
            if (request.MediaFile == null || request.MediaFile.Length == 0)
            {
                return BadRequest("No media file provided.");
            }

            try
            {
                var result = await _contentService.AddContentAsync(request);
                return Ok(result); // Just return the created content directly
            }
            catch (Exception ex)
            {
                // Log the error if necessary
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Get photo by its ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhoto(string id)
        {
            try
            {
                var photo = await _contentService.GetPhotoByIdAsync(id);
                return photo == null ? NotFound() : Ok(photo);
            }
            catch (Exception ex)
            {
                // Log the error if necessary
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Get all photos by userId
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPhotosByUser(string userId)
        {
            try
            {
                var photos = await _contentService.GetPhotosByUserAsync(userId);
                return Ok(photos);
            }
            catch (Exception ex)
            {
                // Log the error if necessary
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}