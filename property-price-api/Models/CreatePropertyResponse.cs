using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
    public class CreatePropertyResponse : CreatePropertyRequest
    {
        public CreatePropertyResponse()
        {
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public string AvatarUrl { get; set; }
    }
}

