using Newtonsoft.Json;

namespace property_price_cosmos_db.Models;

public class Transaction(Guid id, Guid userId, decimal amount, string description, bool completed)
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; } = id;

    [JsonProperty(PropertyName = "userId")]
    public Guid UserId { get; set; } = userId;

    [JsonProperty(PropertyName = "amount")]
    public decimal Amount { get; set; } = amount;

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; } = description;

    [JsonProperty(PropertyName = "isComplete")]
    public bool Completed { get; set; } = completed;

    [JsonProperty(PropertyName = "comments")]
    public List<Comment> Comments { get; set; } = new();
}