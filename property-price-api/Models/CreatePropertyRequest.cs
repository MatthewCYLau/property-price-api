using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
	public class CreatePropertyRequest
	{
        [BsonElement("url")]
        public string ListingUrl { get; set; } = null!;

        public decimal AskingPrice { get; set; }

        public string Address { get; set; } = null!;

    }
}

