using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
	public class Notification: BaseModel
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public bool ReadStatus { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PriceSuggestionId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ActorId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string NotifierId { get; set; }

        public string NotificationType { get; set; } = null!;

        public Notification(
            bool readStatus,
            string priceSuggestionId,
            string actorId,
            string notifierId,
            string notificationType)
        {
            ReadStatus = readStatus;
            PriceSuggestionId = priceSuggestionId;
            ActorId = actorId;
            NotifierId = notifierId;
            NotificationType = notificationType;
        }
    }
}

