namespace property_price_cosmos_db.Models
{
    public class Post
    {
        public int userId { get; set; }
        public int id { get; set; }
        public required string title { get; set; }
        public required string body { get; set; }
    }
}

