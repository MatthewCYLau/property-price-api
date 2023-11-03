using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotifications();
        Task CreateNotification(Notification notification);
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


    }
}

