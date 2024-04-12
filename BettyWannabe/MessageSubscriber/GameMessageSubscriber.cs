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
    public class GameMessageSubscriber : BaseMessageSubscriber
    {
        public GameMessageSubscriber(IMessageResponseService messageResponseService, IConsoleService console)
            : base(messageResponseService, console, "gameOutcomeQueue")
        {
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
    }
}
