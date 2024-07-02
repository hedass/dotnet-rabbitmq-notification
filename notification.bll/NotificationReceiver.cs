using notification.api;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Data.Common;

namespace notification.bll
{
    public class NotificationReceiver : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public NotificationReceiver(string hostName, string userName, string password)
        {
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

        public void ConsumeMessages(string queueName, Action<string> messageHandler)
        {
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                messageHandler(message);
            };

            _channel.BasicConsume(queue: queueName,
                                  autoAck: true,
                                  consumer: consumer);
        }
    }
}
