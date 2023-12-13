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

        public IMongoCollection<Property> Properties => _database.GetCollection<Property>(CollectionNames.PropertiesCollection);
        public IMongoCollection<User> Users => _database.GetCollection<User>(CollectionNames.UsersCollection);
        public IMongoCollection<PriceSuggestion> PriceSuggestions => _database.GetCollection<PriceSuggestion>(CollectionNames.PriceSuggestionsCollection);
        public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>(CollectionNames.NotificationsCollection);
        public IMongoCollection<IngestJob> IngestJobs => _database.GetCollection<IngestJob>(CollectionNames.IngestJobsCollection);
    }
}

