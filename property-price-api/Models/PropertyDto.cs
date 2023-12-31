﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
	public class PropertyDto: BaseModel
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("url")]
        public string ListingUrl { get; set; } = null!;

        public int AskingPrice { get; set; }

        public string Address { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public UserDto? UserDto { get; set; }

        public string AvatarUrl { get; set; }
    }
}

