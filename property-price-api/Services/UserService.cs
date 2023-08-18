using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using property_price_api.Models;

namespace property_price_api.Services
{
	public class UserService
	{
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMapper _mapper;

        public UserService(
        IOptions<PropertyPriceApiDatabaseSettings> propertyPriceApiDatabaseSettings,
        IMapper mapper)
        {
            var mongoClient = new MongoClient(
                propertyPriceApiDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                propertyPriceApiDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                propertyPriceApiDatabaseSettings.Value.UsersCollectionName);

            _mapper = mapper;
        }

        public async Task<List<User>> GetAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public async Task<User>CreateAsync(CreateUserDto createUserDto)
        {
            var _user = _mapper.Map<User>(createUserDto);
            await _usersCollection.InsertOneAsync(_user);
            return _user;
        }
          
    }
}

