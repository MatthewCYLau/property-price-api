using Newtonsoft.Json;

namespace property_price_cosmos_db.Models;

public class Transaction()
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }

    [JsonProperty(PropertyName = "userId")]
    public required Guid UserId { get; set; }

    [JsonProperty(PropertyName = "amount")]
    public required decimal Amount { get; set; }

    [JsonProperty(PropertyName = "description")]
    public required string Description { get; set; }

    [JsonProperty(PropertyName = "isComplete")]
    public required bool Completed { get; set; }

    [JsonProperty(PropertyName = "comments")]
    public required List<Comment> Comments { get; set; }
}