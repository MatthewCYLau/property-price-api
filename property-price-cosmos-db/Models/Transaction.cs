using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace property_price_cosmos_db.Models;

public class Transaction()
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }

    [JsonProperty(PropertyName = "created")]
    public DateTime Created { get; set; } = DateTime.Now;

    [JsonProperty(PropertyName = "modified")]
    public DateTime Modified { get; set; } = DateTime.Now;

    [JsonProperty(PropertyName = "userId")]
    public required Guid UserId { get; set; }

    [JsonProperty(PropertyName = "amount")]
    public required decimal Amount { get; set; }

    [JsonProperty(PropertyName = "description")]
    public required string Description { get; set; }

    [JsonProperty(PropertyName = "isComplete")]
    public required bool Completed { get; set; }

    [JsonProperty(PropertyName = "transactionType")]
    [JsonConverter(typeof(StringEnumConverter))]
    public required TransactionType TransactionType { get; set; }

    [JsonProperty(PropertyName = "comments")]
    public List<Comment> Comments { get; set; } = [];
}