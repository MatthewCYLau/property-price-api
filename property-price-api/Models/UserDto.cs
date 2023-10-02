using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
	public record UserDto
	{
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Email { get; set; } = null!;

        public string UserType { get; set; } = null!;

        public UserDto(string? id, string email, string userType)
        {
            Id = id;
            Email = email;
            UserType = userType;
        }
    }
}

