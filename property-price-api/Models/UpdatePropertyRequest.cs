namespace property_price_api.Models
{
	public class UpdatePropertyRequest
    {
        public string ListingUrl { get; set; } = null!;

        public int AskingPrice { get; set; }

        public string Address { get; set; } = null!;

        public string AvatarUrl { get; set; }
    }
}

