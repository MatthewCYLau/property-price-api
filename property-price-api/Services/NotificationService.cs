using System.Linq.Expressions;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotifications(string? notifierId);
        Task CreateNotification(Notification notification);
        Task<Notification> UpdateNotificationById(string id, UpdateNotificationRequest request);
        Task<Notification> GetNotificationById(string id);
        Task<List<Notification>> GetNotificationsForCurrentUser(bool? readStatus);
    }

    public class NotificationService: INotificationService
	{

        private readonly MongoDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(MongoDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<List<Notification>> GetNotifications(string? notifierId)
        {

            Expression<Func<Notification, bool>> expression;
            if (notifierId is not null)
            {
                expression = x => x.NotifierId == notifierId;
            }

            else
            {
                expression = _ => true;
            }

            return await _context.Notifications.Find(expression).ToListAsync();

        }

        public async Task<List<Notification>> GetNotificationsForCurrentUser(bool? readStatus)
        {

            var httpContext = _httpContextAccessor.HttpContext;
            var _userDto = (Task<UserDto>)httpContext.Items["User"];

            Expression<Func<Notification, bool>> expression;
            if (readStatus is not null)
            {
                expression = x => x.NotifierId == _userDto.Result.Id && x.ReadStatus == readStatus;
            }

            else
            {
                expression = x => x.NotifierId == _userDto.Result.Id;
            }

            return await _context.Notifications.Find(expression).ToListAsync();

        }

        public async Task CreateNotification(Notification notification)
        {
            await _context.Notifications.InsertOneAsync(notification);
        }

        public async Task<Notification> UpdateNotificationById(string id, UpdateNotificationRequest request)
        {
            var filter = Builders<Notification>.Filter.Where(x => x.Id == id);
            var update = Builders<Notification>.Update.Set(x => x.ReadStatus, request.ReadStatus);
            var options = new FindOneAndUpdateOptions<Notification>() { ReturnDocument = ReturnDocument.After };
            var notification = await _context.Notifications.FindOneAndUpdateAsync(filter, update, options);
            return notification;
        }

        public async Task<Notification> GetNotificationById(string id)
        {
            return await _context.Notifications.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}

