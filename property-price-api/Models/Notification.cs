using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
	public class Notification: BaseModel
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string ReadStatus { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string PriceSuggestionId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ActorId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string NotifierId { get; set; }

        public string NotificationType { get; set; } = null!;
    }
}

