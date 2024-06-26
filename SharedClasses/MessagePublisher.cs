﻿using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using System.Threading.Tasks;
using SharedClasses.Interface;

namespace SharedClasses
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly ConnectionFactory factory;

        public MessagePublisher()
        {
            this.factory = new ConnectionFactory() { HostName = "localhost" };
        }

        public async Task PublishMessageAsync<T>(T message, string queueName)
        {
            await Task.Run(() =>
            {
                using (var connection = this.factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var json = JsonSerializer.Serialize(message);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: null,
                                         body: body);
                }
            });
        }

        public void PublishMessage<T>(T message, string queueName)
        {
            using var connection = this.factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
