using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using System;
using System.Text;
using System.Text.Json;
using GameService.Interface;

namespace GameService.Messaging
{
    public class BetMessageSubscriber
    {
        private readonly IGameService gameService;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _queueName = "betQueue";
        private readonly MessagePublisher messagePublisher;

        public BetMessageSubscriber(IGameService gameService, MessagePublisher publisher)
        {
            this.gameService = gameService;
            this.messagePublisher = publisher;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var betMessage = JsonSerializer.Deserialize<BetMessage>(messageJson);

                if (betMessage != null)
                {
                    try
                    {
                        await this.gameService.PlayBetAsync(betMessage);
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        var message = new WalletBalanceUpdateMessage
                        {
                            CurrentBalance = betMessage.CurrentBalance,
                            IsSuccessful = false,
                            Message = $"{ex.Message}"
                        };

                        await this.messagePublisher.PublishMessageAsync<WalletBalanceUpdateMessage>(message, "gameOutcomeQueue");
                    }
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        }

        public void StopListening()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
