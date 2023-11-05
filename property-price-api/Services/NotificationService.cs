using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotifications();
        Task CreateNotification(Notification notification);
        Task<Notification> UpdateNotificationById(string id, UpdateNotificationRequest request);
        Task<Notification> GeNotificationById(string id);
    }

    public class NotificationService: INotificationService
	{

        private readonly MongoDbContext _context;

        public NotificationService(MongoDbContext context)
        {
            _context = context;
        }


        public async Task<List<Notification>> GetNotifications()
        {
            var notifications = await _context.Notifications.Find(_ => true).ToListAsync();
            return notifications;
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

        public async Task<Notification> GeNotificationById(string id)
        {
            return await _context.Notifications.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}

