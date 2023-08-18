using Microsoft.Extensions.Options;
using MongoDB.Driver;
using property_price_api.Models;

namespace property_price_api.Services
{
	public class UserService
	{
        private readonly IMongoCollection<User> _usersCollection;


        public UserService(
        IOptions<PropertyPriceApiDatabaseSettings> propertyPriceApiDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                propertyPriceApiDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                propertyPriceApiDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                propertyPriceApiDatabaseSettings.Value.UsersCollectionName);
        }

        public async Task<List<User>> GetAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public async Task CreateAsync(User user) =>
          await _usersCollection.InsertOneAsync(user);
    }
}

