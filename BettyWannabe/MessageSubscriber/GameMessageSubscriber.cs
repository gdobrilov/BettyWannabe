//using System;
//using System.Text;
//using System.Text.Json;
//using Microsoft.Extensions.Hosting;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using SharedClasses;

//namespace BettyWannabe.MessageSubscriber
//{
//	public class GameMessageSubscriber : BackgroundService
//	{
//        private readonly MessageResponseService messageResponseService;

//        public GameMessageSubscriber(MessageResponseService messageResponseService)
//        {
//            this.messageResponseService = messageResponseService;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            var factory = new ConnectionFactory() { HostName = "localhost" };
//            using var connection = factory.CreateConnection();
//            using var channel = connection.CreateModel();
//            {
//                channel.QueueDeclare(queue: "gameOutcomeQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

//                var consumer = new EventingBasicConsumer(channel);
//                consumer.Received += (model, ea) =>
//                {
//                    var body = ea.Body.ToArray();
//                    var message = Encoding.UTF8.GetString(body);
//                    var gameMessage = JsonSerializer.Deserialize<WalletBalanceUpdateMessage>(message);
//                    Console.WriteLine($"{message}");
//                    if (gameMessage != null)
//                    {
//                        messageResponseService.GameOutcomeReceived.TrySetResult(message);
//                    }
//                };

//                channel.BasicConsume(queue: "gameOutcomeQueue", autoAck: true, consumer: consumer);

//                while (!stoppingToken.IsCancellationRequested)
//                {
//                    await Task.Delay(1000, stoppingToken); // Keep the task alive
//                }
//            }
//        }
//    }
//}


using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using System.Threading;
using System.Threading.Tasks;

namespace BettyWannabe.MessageSubscriber
{
    public class GameMessageSubscriber : BackgroundService
    {
        private readonly MessageResponseService _messageResponseService;
        private IConnection _connection;
        private IModel _channel;
        private string _hostname = "localhost"; // Consider moving to configuration
        private string _queueName = "gameOutcomeQueue"; // Consider moving to configuration

        public GameMessageSubscriber(MessageResponseService messageResponseService)
        {
            _messageResponseService = messageResponseService;
            InitializeRabbitMQListener();
        }

        private void InitializeRabbitMQListener()
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var gameMessage = JsonSerializer.Deserialize<WalletBalanceUpdateMessage>(message);
                if (gameMessage != null)
                {
                    Console.WriteLine(gameMessage.Message);
                    _messageResponseService.GameOutcomeReceived.TrySetResult(message);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }
    }
}
