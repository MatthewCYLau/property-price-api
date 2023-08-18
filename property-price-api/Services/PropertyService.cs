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
        await _propertiesCollection.Aggregate()
            .Lookup("users", "UserId", "_id", @as: "User")
            .Unwind("User")
            .As<Property>()
            .ToListAsync();

        public async Task<Property?> GetAsync(string id) =>
            await _propertiesCollection.Aggregate()
            .Match(x => x.Id == id)
            .Lookup("users", "UserId", "_id", @as: "User")
            .Unwind("User")
            .As<Property>()
            .SingleOrDefaultAsync();

        public async Task CreateAsync(Property property) =>
            await _propertiesCollection.InsertOneAsync(property);

        public async Task UpdateAsync(string id, Property property) =>
            await _propertiesCollection.ReplaceOneAsync(x => x.Id == id, property);

        public async Task RemoveAsync(string id) =>
            await _propertiesCollection.DeleteOneAsync(x => x.Id == id);
    }
}

