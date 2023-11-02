using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface INotificationService
    {
        Task CreateNotification(Notification notification);
    }

    public class NotificationService: INotificationService
	{

        private readonly MongoDbContext _context;

        public NotificationService(MongoDbContext context)
        {
            _context = context;
        }


        public async Task CreateNotification(Notification notification)
        {
            await _context.Notifications.InsertOneAsync(notification);
        }


    }
}

