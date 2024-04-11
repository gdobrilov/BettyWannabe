using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using System.Threading;
using System.Threading.Tasks;
using SharedClasses.Interface;
using BettyWannabe.Interface;

namespace BettyWannabe.MessageSubscriber
{
    public class WalletMessageSubscriber : BackgroundService
    {
        private readonly IMessagePublisher messagePublisher;
        private readonly IMessageResponseService messageResponseService;
        private readonly IConsoleService consoleService;
        private IConnection connection;
        private IModel channel;
        private readonly string hostname = "localhost";
        private readonly string queueName = "walletBalanceUpdateQueue";

        public WalletMessageSubscriber(IMessagePublisher messagePublisher, IMessageResponseService messageResponseService, IConsoleService console)
        {
            this.messagePublisher = messagePublisher;
            this.messageResponseService = messageResponseService;
            this.consoleService = console;
            this.InitializeRabbitMQListener();
        }

        private void InitializeRabbitMQListener()
        {
            var factory = new ConnectionFactory() { HostName = this.hostname };
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
                var message = Encoding.UTF8.GetString(body);

                var walletMessage = JsonSerializer.Deserialize<WalletBalanceUpdateMessage>(message);
                if (walletMessage != null)
                {
                    if (walletMessage.ShouldPlaceBet && walletMessage.IsSuccessful)
                    {
                        var betMessage = new BetMessage
                        {
                            Amount = walletMessage.Amount,
                            CurrentBalance = walletMessage.CurrentBalance
                        };

                        await this.messagePublisher.PublishMessageAsync<BetMessage>(betMessage, "betQueue");
                    }
                    else
                    {
                        if (walletMessage.ShouldPlaceBet)
                        {
                            this.consoleService.WriteLine($"Your bet of {walletMessage.Amount} was not successful due to insufficient balance. Your balance is {walletMessage.CurrentBalance}.");
                        }
                        else
                        {
                            this.consoleService.WriteLine(walletMessage.Message);
                        }
                        this.messageResponseService.GameOutcomeReceived.TrySetResult(message);
                    }

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
                this.connection.Close();
            }
            base.Dispose();
        }
    }
}
