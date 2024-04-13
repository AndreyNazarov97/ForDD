using ForDD.Domain.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;

namespace ForDD.Consumer
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IOptions<RabbitMqSettings> _options;

        public RabbitMqListener(IOptions<RabbitMqSettings> options)
        {
            _options = options;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_options.Value.QueueName, true, false, false, null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, basicDeliver) =>
            {
                var content = Encoding.UTF8.GetString(basicDeliver.Body.ToArray());

                Debug.WriteLine($"Message received: {content}");

                _channel.BasicAck(basicDeliver.DeliveryTag, false);
            };

            _channel.BasicConsume(_options.Value.QueueName, false, consumer);

            

            return Task.CompletedTask;
        }
    }
}
