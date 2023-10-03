using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
	public class PriceSuggestion
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Created { get; set; }

        [Range(-100, 100)]
        public int DifferenceInPercentage { get; set; }
        
        [Required(ErrorMessage = "Note is required")]
        public string Note { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? PropertyId { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        
        public Property? Property { get; set; }
    }
}

