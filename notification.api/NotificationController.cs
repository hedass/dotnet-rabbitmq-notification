using Microsoft.AspNetCore.Mvc;
using notification.bll;

namespace notification.api
{
    [ApiController]
    [Route("notification")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly NotificationReceiver _notificationReceiver;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] string message)
        {
            await _notificationService.SendNotificationAsync(message);
            return Ok();
        }
    }
}
