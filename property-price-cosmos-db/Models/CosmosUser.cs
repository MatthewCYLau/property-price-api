using Newtonsoft.Json;

namespace property_price_cosmos_db.Models;

public class CosmosUser()
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }

    [JsonProperty(PropertyName = "name")]
    public required string Name { get; set; }

    [JsonProperty(PropertyName = "dateOfBirth")]
    public required DateTime DateOfBirth { get; set; }
}
