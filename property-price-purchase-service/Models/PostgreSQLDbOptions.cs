namespace property_price_purchase_service.Models;

public class PostgreSqlDbOptions
{
    public const string PostgreSqlDbSettingsName = "PostgreSqlDbSettings";
    public string ConnectionString { get; set; } = null;
}