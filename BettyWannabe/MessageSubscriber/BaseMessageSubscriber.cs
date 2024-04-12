using System;
using BettyWannabe.Interface;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using SharedClasses.Interface;

namespace BettyWannabe.MessageSubscriber
{
	public class BaseMessageSubscriber : BackgroundService
    {
        private string hostname = "localhost";
        public IConnection connection;
        public string queueName;
        public readonly IConsoleService consoleService;
        public readonly IMessageResponseService messageResponseService;
        public IModel channel;

        public BaseMessageSubscriber(IMessageResponseService messageResponseService, IConsoleService console, string queue)
		{
            this.messageResponseService = messageResponseService;
            this.consoleService = console;
            this.queueName = queue;
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

