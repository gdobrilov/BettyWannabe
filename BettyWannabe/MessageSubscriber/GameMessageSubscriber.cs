using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using System.Threading;
using System.Threading.Tasks;
using BettyWannabe.Interface;

namespace BettyWannabe.MessageSubscriber
{
    public class GameMessageSubscriber : BackgroundService
    {
        private readonly IMessageResponseService messageResponseService;
        private readonly IConsoleService consoleService;
        private IConnection connection;
        private IModel channel;
        private string hostname = "localhost";
        private string queueName = "gameOutcomeQueue";

        public GameMessageSubscriber(IMessageResponseService messageResponseService, IConsoleService console)
        {
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
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var gameMessage = JsonSerializer.Deserialize<WalletBalanceUpdateMessage>(message);

                if (gameMessage != null)
                {
                    this.consoleService.WriteLine(gameMessage.Message);
                    this.messageResponseService.GameOutcomeReceived.TrySetResult(message);
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
