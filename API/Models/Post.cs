namespace API.Models
{
    public class Post : Common
    {
        public required User Auther { get; set; }
        public string? Topic { get; set; }
        public required string Header { get; set; }
        public required string Body { get; set; }
    }

    public class PostDTO
    {
        public required int AutherId { get; set; }
        public string? Topic { get; set; }
        public required string Header { get; set; }
        public required string Body { get; set; }
    }
}
