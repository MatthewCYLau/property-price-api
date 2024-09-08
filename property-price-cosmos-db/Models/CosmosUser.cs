using Newtonsoft.Json;

namespace property_price_cosmos_db.Models;

public class CosmosUser(Guid id, string name)
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; } = id;

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; } = name;
}
