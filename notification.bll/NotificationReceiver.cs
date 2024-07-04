using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights;

namespace notification.bll
{
    public class NotificationReceiver : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly TelemetryClient _telemetryClient;

        public NotificationReceiver(string hostName, string userName, string password, TelemetryClient telemetryClient)
        {
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _telemetryClient = telemetryClient;
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

                var dependencyTelemetry = new DependencyTelemetry
                {
                    Type = "RabbitMQ",
                    Target = queueName,
                    Data = message,
                    Name = "Receive Message"
                };
                var operation = _telemetryClient.StartOperation(dependencyTelemetry);

                try
                {

                    _telemetryClient.TrackDependency(dependencyTelemetry);

                    messageHandler(message);

                    operation.Telemetry.Success = true;
                }
                catch (Exception ex)
                {
                    operation.Telemetry.Success = false;
                    _telemetryClient.TrackException(ex); 
                    _telemetryClient.TrackDependency(dependencyTelemetry);
                }
                finally
                {
                    _telemetryClient.StopOperation(operation);
                }
            };

            _channel.BasicConsume(queue: queueName,
                                  autoAck: true,
                                  consumer: consumer);
        }
    }
}
