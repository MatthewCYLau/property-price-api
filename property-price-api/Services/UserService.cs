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

        public async Task<List<UserDto>> GetAsync()
        {
            var _users = await _usersCollection.Find(_ => true).ToListAsync();
            var _usersDto = _mapper.Map<List<UserDto>>(_users);
            return _usersDto;
        }
            

        public async Task<UserDto>CreateAsync(CreateUserDto createUserDto)
        {
            var _user = _mapper.Map<User>(createUserDto);
            await _usersCollection.InsertOneAsync(_user);
            var _createdUser = _mapper.Map<UserDto>(_user);

            return _createdUser;
        }
          
    }
}

