using Newtonsoft.Json;

namespace property_price_api.Models;

public class ImportPropertiesFromCloudStorageRequest(string ObjectUrl)
{
    [JsonProperty(PropertyName = "objectUrl")]
    public string ObjectUrl { get; set; } = ObjectUrl;
}
