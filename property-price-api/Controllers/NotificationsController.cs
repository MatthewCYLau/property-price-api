using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {

        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpGet]
        public async Task<List<Notification>> Get() =>
            await _notificationService.GetNotifications();

        [Authorize]
        [HttpPatch("{id:length(24)}")]
        public async Task<ActionResult<Notification>> UpdateNotificationById(string id, UpdateNotificationRequest updateNotificationRequest)
        {
            //var _userDto = await _userService.GetUserById(id);

            //if (_userDto is null)
            //{
            //    return NotFound();
            //}

            var notification = await _notificationService.UpdateNotificationById(id, updateNotificationRequest);
            return Ok(notification);
        }
    }
}

