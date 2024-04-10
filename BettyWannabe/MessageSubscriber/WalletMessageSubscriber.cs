//using System;
//using System.Text;
//using System.Text.Json;
//using Microsoft.Extensions.Hosting;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using SharedClasses;

//namespace BettyWannabe.MessageSubscriber
//{
//	public class WalletMessageSubscriber : BackgroundService
//    {
//        private MessagePublisher messagePublisher;

//        public WalletMessageSubscriber(MessagePublisher publisher)
//        {
//            this.messagePublisher = publisher;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            var factory = new ConnectionFactory() { HostName = "localhost" };
//            using var connection = factory.CreateConnection();
//            using var channel = connection.CreateModel();
//            {

//                channel.QueueDeclare(queue: "walletBalanceUpdateQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

//                var consumer = new EventingBasicConsumer(channel);
//                consumer.Received += (model, ea) =>
//                {
//                    var body = ea.Body.ToArray();
//                    var message = Encoding.UTF8.GetString(body);
//                    var gameMessage = JsonSerializer.Deserialize<WalletBalanceUpdateMessage>(message);

//                    if (gameMessage.ShouldPlaceBet)
//                    {
//                        var betMessage = new BetMessage()
//                        {
//                            Amount = gameMessage.Amount,
//                            CurrentBalance = gameMessage.CurrentBalance
//                        };

//                        this.messagePublisher.PublishMessage<BetMessage>(betMessage, "betQueue");
//                    }
//                    else
//                    {
//                        Console.WriteLine($"{gameMessage.Message}");
//                    }
//                };

//                channel.BasicConsume(queue: "walletBalanceUpdateQueue", autoAck: true, consumer: consumer);

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
    public class WalletMessageSubscriber : BackgroundService
    {
        private readonly MessagePublisher _messagePublisher;
        private readonly MessageResponseService _messageResponseService;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _hostname = "localhost"; // Ideally, from configuration
        private readonly string _queueName = "walletBalanceUpdateQueue"; // Ideally, from configuration

        public WalletMessageSubscriber(MessagePublisher messagePublisher, MessageResponseService messageResponseService)
        {
            _messagePublisher = messagePublisher;
            _messageResponseService = messageResponseService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            InitializeRabbitMQListener();

            stoppingToken.Register(() =>
            {
                _channel?.Close();
                _connection?.Close();
            });

            // Keep the task alive until cancellation is requested.
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task InitializeRabbitMQListener()
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var walletMessage = JsonSerializer.Deserialize<WalletBalanceUpdateMessage>(message);
                if (walletMessage != null)
                {
                    if (walletMessage.ShouldPlaceBet)
                    {
                        var betMessage = new BetMessage
                        {
                            Amount = walletMessage.Amount,
                            CurrentBalance = walletMessage.CurrentBalance
                        };

                        await _messagePublisher.PublishMessageAsync<BetMessage>(betMessage, "betQueue");
                    }
                    else
                    {
                        Console.WriteLine(walletMessage.Message);
                        _messageResponseService.GameOutcomeReceived.TrySetResult(message);
                    }

                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }
    }
}
