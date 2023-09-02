using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
    public class User
    {
        public User(string? id, string email, string password)
        {
            Id = id;
            Email = email;
            Password = password;
        }

        public User()
        {

        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Email { get; set; } = null!;

        [JsonIgnore]
        public string Password { get; set; } = null!;

    }
}

