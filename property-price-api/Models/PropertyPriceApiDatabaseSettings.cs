namespace property_price_api.Models
{
    public class PropertyPriceApiDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string PropertiesCollectionName { get; set; } = null!;

        public string UsersCollectionName { get; set; } = null!;

    }
}

