using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using System.Threading;
using System.Threading.Tasks;
using GameService.Interface;
using SharedClasses.Interface;

namespace GameService.Messaging
{
    public class BetMessageSubscriber : BackgroundService
    {
        private readonly IGameService gameService;
        private readonly IMessagePublisher messagePublisher;
        private IConnection connection;
        private IModel channel;
        private readonly string queueName = "betQueue";

        public BetMessageSubscriber(IGameService gameService, IMessagePublisher publisher)
        {
            this.gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            this.messagePublisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            InitializeRabbitMQListener();
        }

        private void InitializeRabbitMQListener()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            this.connection = factory.CreateConnection();
            this.channel = this.connection.CreateModel();
            this.channel.QueueDeclare(queue: this.queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(this.channel);
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
                        this.channel.BasicAck(ea.DeliveryTag, false);
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

            this.channel.BasicConsume(queue: this.queueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (this.channel.IsOpen)
            {
                this.channel.Close();
                this.connection.Close();
            }
            base.Dispose();
        }
    }
}
