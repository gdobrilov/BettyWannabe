using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using System.Threading;
using System.Threading.Tasks;
using WalletService.Interfaces;
using SharedClasses.Interface;

namespace WalletService.Messaging
{
    public class MessageSubscriber : BackgroundService
    {
        private readonly IWalletService walletService;
        private readonly IMessagePublisher messagePublisher;
        private IConnection connection;
        private IModel channel;
        private readonly string queueName = "walletQueue";

        public MessageSubscriber(IWalletService walletService, IMessagePublisher messagePublisher)
        {
            this.walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            this.messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
            InitializeRabbitMQListener();
        }

        private void InitializeRabbitMQListener()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            this.connection = factory.CreateConnection();
            this.channel = this.connection.CreateModel();
            this.channel.QueueDeclare(queue: this.queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        private async Task HandleMessageProcessing(WalletUpdateMessage message)
        {
            if (message.IsDeposit)
            {
                var response = this.walletService.Deposit(message);

                if (message.IsBet)
                {
                    await this.messagePublisher.PublishMessageAsync<WalletBalanceUpdateMessage>(response, "gameOutcomeQueue");
                }
                else
                {
                    await this.messagePublisher.PublishMessageAsync<WalletBalanceUpdateMessage>(response, "walletBalanceUpdateQueue");
                }
            }
            else
            {
                var response = this.walletService.Withdraw(message);

                await this.messagePublisher.PublishMessageAsync<WalletBalanceUpdateMessage>(response, "walletBalanceUpdateQueue");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<WalletUpdateMessage>(json);

                if (message != null)
                {
                    await this.HandleMessageProcessing(message);
                }
            };

            this.channel.BasicConsume(queue: this.queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (this.channel.IsOpen)
            {
                this.channel.Close();
            }
            this.connection?.Close();
            base.Dispose();
        }
    }
}
