using MongoDB.Driver;
using property_price_api.Models;

namespace property_price_api.Data
{
	public class MongoDbContext
	{
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Property> Properties => _database.GetCollection<Property>("properties");
        public IMongoCollection<User> Users => _database.GetCollection<User>("users");

    }
}

