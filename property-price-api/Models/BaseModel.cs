using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models;

public class BaseModel
{
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Created { get; set; }
}