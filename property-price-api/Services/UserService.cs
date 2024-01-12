using BC = BCrypt.Net.BCrypt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Helpers;
using property_price_api.Models;
using System.Linq.Expressions;

namespace property_price_api.Services
{

    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        Task<List<UserDto>> GetUsers();
        Task<AuthenticateResponse> CreateUser(CreateUserRequest createUserDto);
        Task<UserDto> GetUserByEmail(string email);
        Task<UserDto> GetUserById(string id);
        Task<UserDto> GetCurrentUser();
        Task<bool> DeleteUser(string id);
        Task<bool> UpdateUserById(string id, UpdateUserRequest updateUserRequest);
        Task<UsersStatisticsResponse> GetUsersStatistics();
    }

    public class UserService: IUserService
	{
        private readonly ILogger _logger;
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            ILogger<UserService> logger,
            MongoDbContext context,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<UserDto>> GetUsers()
        {
            var users = await _context.Users.Find(_ => true).ToListAsync();
            var usersDto = _mapper.Map<List<UserDto>>(users);
            return usersDto;
        }

        public async Task<UserDto> GetUserById(string id)
        {
            var _user = await _context.Users.Find(x => x.Id == id).FirstOrDefaultAsync();
            var userDto = _mapper.Map<UserDto>(_user);
            return userDto;
        }

        public async Task<UserDto> GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var _userDto = (Task<UserDto>)httpContext.Items["User"];
            var _user = await _context.Users.Find(x => x.Id == _userDto.Result.Id).FirstOrDefaultAsync();
            var userDto = _mapper.Map<UserDto>(_user);
            return userDto;
        }


        public async Task<AuthenticateResponse> CreateUser(CreateUserRequest createUserRequest)
        {
            var user = _mapper.Map<User>(createUserRequest);
            user.Password = BC.HashPassword(user.Password);
            user.Created = DateTime.Now;
            await _context.Users.InsertOneAsync(user);
            var token = GenerateJwtToken(user);
            return new AuthenticateResponse(user, token);
        }

        public async Task<UserDto> GetUserByEmail(string email)
        {
            var _user = await _context.Users.Find(x => x.Email == email).FirstOrDefaultAsync();
            var _userDto = _mapper.Map<UserDto>(_user);
            return _userDto;
        }


        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            if (model.Email == UserConstants.PlaceholderUserEmail)
            {
                throw new CustomException("Authentication via placeholder user not allowed!");
            }

            var _user = await _context.Users.Find(x => x.Email == model.Email).FirstOrDefaultAsync();

            if (_user == null || !BC.Verify(model.Password, _user.Password))
            {
                return null;
            }

            var token = GenerateJwtToken(_user);

            return new AuthenticateResponse(_user, token);
        }

        public async Task<bool> DeleteUser(string id)
        {
            if (!ValidatePermission(id)){
                return false;
            }
            await _context.Users.DeleteOneAsync(x => x.Id == id);
            return true;
        }

        public async Task<bool> UpdateUserById(string id, UpdateUserRequest updateUserRequest)
        {

            if (!ValidatePermission(id))
            {
                _logger.LogWarning("User does not have permission to update user {0}", id);
                return false;
            }

            var filter = Builders<User>.Filter.Where(x => x.Id == id);
            var update = Builders<User>.Update.Set(x => x.UserType, updateUserRequest.UserType);

            if (updateUserRequest.Password != null)
            {
                update = Builders<User>.Update.Set(x => x.Password, BC.HashPassword(updateUserRequest.Password));
            }

            var options = new FindOneAndUpdateOptions<User>();
            await _context.Users.FindOneAndUpdateAsync(filter, update, options);

            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? _appSettings.Secret;
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        private bool ValidatePermission(string id)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var _userDto = (Task<UserDto>)httpContext.Items["User"];
            return _userDto.Result.Id == id;
        }

        public async Task<UsersStatisticsResponse> GetUsersStatistics()
        {
            var renterCount = await GetUsersStatisticsByExpression(x => x.UserType == UserTypes.Renter);

            return new UsersStatisticsResponse(
                (int)renterCount
                );
        }

        private async Task<long> GetUsersStatisticsByExpression(Expression<Func<User, bool>> expression)
        {
            return await _context.Users.CountDocumentsAsync(Builders<User>.Filter.Where(expression));
        }
    }
}

