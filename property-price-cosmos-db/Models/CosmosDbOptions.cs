namespace property_price_cosmos_db.Models;

public class CosmosDbOptions
{
    public const string CosmosDbSettingsName = "CosmosDbSettings";
    public string ConnectionString { get; set; } = null!;
    public string TransactionsContainerId { get; set; } = null!;
    public string UsersContainerId { get; set; } = null!;
    public string PaymentRequestsContainerId { get; set; } = null!;
    public string DatabaseId { get; set; } = null!;
}