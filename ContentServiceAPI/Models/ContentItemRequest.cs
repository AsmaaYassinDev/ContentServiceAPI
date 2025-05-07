namespace ContentServiceAPI.Models
{
    public class ContentItemRequest
    {
        public string userId { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public string Location { get; set; }
        public List<string> Tags { get; set; }
        public IFormFile MediaFile { get; set; }
    }
}
