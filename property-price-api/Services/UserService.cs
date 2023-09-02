using BC = BCrypt.Net.BCrypt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{

    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        Task<List<UserDto>> GetUsers();
        Task<UserDto> CreateUser(CreateUserDto createUserDto);
        Task<UserDto> GetUserByEmail(string email);
        Task<UserDto> GetUserById(string id);
        Task<bool> DeleteUser(string id);
    }

    public class UserService: IUserService
	{
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            MongoDbContext context,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<UserDto>> GetUsers()
        {
            var _users = await _context.Users.Find(_ => true).ToListAsync();
            var _usersDto = _mapper.Map<List<UserDto>>(_users);
            return _usersDto;
        }

        public async Task<UserDto> GetUserById(string id)
        {
            var _user = await _context.Users.Find(x => x.Id == id).FirstOrDefaultAsync();
            var userDto = _mapper.Map<UserDto>(_user);
            return userDto;
        }


        public async Task<UserDto> CreateUser(CreateUserDto createUserDto)
        {
            var _user = _mapper.Map<User>(createUserDto);
            _user.Password = BC.HashPassword(_user.Password);
            await _context.Users.InsertOneAsync(_user);
            var _createdUser = _mapper.Map<UserDto>(_user);

            return _createdUser;
        }

        public async Task<UserDto> GetUserByEmail(string email)
        {
            var _user = await _context.Users.Find(x => x.Email == email).FirstOrDefaultAsync();
            var _userDto = _mapper.Map<UserDto>(_user);
            return _userDto;
        }


        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var _user = await _context.Users.Find(x => x.Email == model.Email).FirstOrDefaultAsync();

            if (_user == null || !BC.Verify(model.Password, _user.Password))
            {
                return null;
            }

            var token = generateJwtToken(_user);

            return new AuthenticateResponse(_user, token);
        }

        public async Task<bool> DeleteUser(string id)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = (Task<User>)httpContext.Items["User"];
            if (user.Result.Id != id)
            {
                return false;
            }
            await _context.Users.DeleteOneAsync(x => x.Id == id);
            return true;
        }
        

        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}

