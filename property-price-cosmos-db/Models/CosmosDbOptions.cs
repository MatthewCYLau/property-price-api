namespace property_price_cosmos_db.Models;

public class CosmosDbOptions
{
    public const string CosmosDbSettingsName = "CosmosDbSettings";
    public string ConnectionString { get; set; } = null!;
    public string ContainerId { get; set; } = null!;
    public string DatabaseId { get; set; } = null!;
}