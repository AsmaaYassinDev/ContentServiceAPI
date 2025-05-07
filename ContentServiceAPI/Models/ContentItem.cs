using System.Text.Json.Serialization;

namespace ContentServiceAPI.Models
{
    public class ContentItem
    {
        [JsonPropertyName("id")]
        public string id { get; set; }
        public string userId { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public string Location { get; set; }
        public List<string> Tags { get; set; }
        public DateTime UploadDate { get; set; }
        public string MediaUrl { get; set; }
    }
}
