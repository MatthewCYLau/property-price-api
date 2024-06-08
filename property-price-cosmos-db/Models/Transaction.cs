using Newtonsoft.Json;

namespace property_price_cosmos_db.Models;

public class Transaction(string? id, decimal amount, string description, bool completed)
{
    [JsonProperty(PropertyName = "id")]
    public string? Id { get; set; } = id;

    [JsonProperty(PropertyName = "amount")]
    public decimal Amount { get; set; } = amount;

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; } = description;

    [JsonProperty(PropertyName = "isComplete")]
    public bool Completed { get; set; } = completed;

    [JsonProperty(PropertyName = "comments")]
    public List<Comment> Comments { get; set; } = new();
}