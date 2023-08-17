using Microsoft.Extensions.Options;
using MongoDB.Driver;
using property_price_api.Models;

namespace property_price_api.Services
{
	public class PropertyService
	{

        private readonly IMongoCollection<Property> _propertiesCollection;


        public PropertyService(
        IOptions<PropertyPriceApiDatabaseSettings> propertyPriceApiDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                propertyPriceApiDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                propertyPriceApiDatabaseSettings.Value.DatabaseName);

            _propertiesCollection = mongoDatabase.GetCollection<Property>(
                propertyPriceApiDatabaseSettings.Value.PropertiesCollectionName);
        }

        public async Task<List<Property>> GetAsync() =>
        await _propertiesCollection.Find(_ => true).ToListAsync();

        public async Task<Property?> GetAsync(string id) =>
            await _propertiesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Property property) =>
            await _propertiesCollection.InsertOneAsync(property);
    }
}

