using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
	public class PriceSuggestion
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public int DifferenceInPercentage { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? PropertyId { get; set; }
    }
}

