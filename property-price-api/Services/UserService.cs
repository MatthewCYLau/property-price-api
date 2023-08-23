using AutoMapper;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
	public class UserService
	{
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;

        public UserService(MongoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAsync()
        {
            var _users = await _context.Users.Find(_ => true).ToListAsync();
            var _usersDto = _mapper.Map<List<UserDto>>(_users);
            return _usersDto;
        }
            

        public async Task<UserDto>CreateAsync(CreateUserDto createUserDto)
        {
            var _user = _mapper.Map<User>(createUserDto);
            await _context.Users.InsertOneAsync(_user);
            var _createdUser = _mapper.Map<UserDto>(_user);

            return _createdUser;
        }
          
    }
}

