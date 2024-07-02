using notification.bll;

namespace notification.api
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string message);
        void StartConsumingMessages();
    }

    public class NotificationService : INotificationService
    {
        private readonly NotificationReceiver _notificationReceiver;

        public NotificationService(NotificationReceiver notificationReceiver) 
        { 
            _notificationReceiver = notificationReceiver;
        }

        public void StartConsumingMessages()
        {
            _notificationReceiver.ConsumeMessages("notificationQueue", (msg) => { this.SendNotificationAsync(msg); });
        }

        public Task SendNotificationAsync(string message)
        {
            // Logic to send an notification
            Console.WriteLine($"Sending notification: {message}");
            return Task.CompletedTask;
        }
    }
}
