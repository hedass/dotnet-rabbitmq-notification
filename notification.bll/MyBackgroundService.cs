using notification.api;
using Microsoft.Extensions.Hosting;

namespace notification.bll
{
    public class  MyBackgroundService : BackgroundService
    {
        private readonly INotificationService _notificationService;

        public MyBackgroundService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _notificationService.StartConsumingMessages();

            // Keep the background service running as long as the application is running
            await Task.Yield();
        }
    }
}
