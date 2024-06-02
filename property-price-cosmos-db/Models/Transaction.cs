using Newtonsoft.Json;

namespace property_price_cosmos_db.Models;

public class Transaction
{
    [JsonProperty(PropertyName = "id")]
    public string? Id { get; set; }
    [JsonProperty(PropertyName = "amount")]
    public decimal Amount { get; set; }
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }
    [JsonProperty(PropertyName = "isComplete")]
    public bool Completed { get; set; }
}