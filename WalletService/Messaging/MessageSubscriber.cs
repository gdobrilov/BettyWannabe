//using System;
//using System.Text;
//using System.Text.Json;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using SharedClasses;
//using WalletService.Interfaces;
//using WalletService.Services;

//namespace WalletService.Messaging
//{
//	public class MessageSubscriber
//	{
//		private IWalletService walletService;
//        private MessagePublisher messagePublisher;

//		public MessageSubscriber(IWalletService walletService, MessagePublisher messagePublisher)
//		{
//			this.walletService = walletService;
//            this.messagePublisher = messagePublisher;
//		}

//		public void StartListening()
//		{
//            var factory = new ConnectionFactory() { HostName = "localhost" };
//            using var connection = factory.CreateConnection();
//            using var channel = connection.CreateModel();

//            channel.QueueDeclare(queue: "walletQueue",
//                                 durable: false,
//                                 exclusive: false,
//                                 autoDelete: false,
//                                 arguments: null);
//            var consumer = new EventingBasicConsumer(channel);
//            consumer.Received += (model, ea) =>
//            {
//                var body = ea.Body.ToArray();
//                var json = Encoding.UTF8.GetString(body);
//                var message = JsonSerializer.Deserialize<WalletUpdateMessage>(json);

//                if (message != null)
//                {
//                    if (message.IsDeposit)
//                    {
//                        var response = this.walletService.Deposit(message);

//                        if (message.IsBet)
//                        {
//                            this.messagePublisher.PublishMessage<WalletBalanceUpdateMessage>(response, "gameOutcomeQueue");
//                        }
//                        else
//                        {
//                            this.messagePublisher.PublishMessage<WalletBalanceUpdateMessage>(response, "walletBalanceUpdateQueue");
//                        }
//                    }
//                    else
//                    {
//                        var response = this.walletService.Withdraw(message);

//                        this.messagePublisher.PublishMessage<WalletBalanceUpdateMessage>(response, "walletBalanceUpdateQueue");

//                    }
//                }
//            };

//            channel.BasicConsume(queue: "walletQueue",
//                            autoAck: true,
//                            consumer: consumer);
//        }
//    }
//}


using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using System;
using System.Text;
using System.Text.Json;
using WalletService.Interfaces;

namespace WalletService.Messaging
{
    public class MessageSubscriber
    {
        private readonly IWalletService _walletService;
        private readonly MessagePublisher _messagePublisher;
        private IConnection _connection;
        private IModel _channel;

        public MessageSubscriber(IWalletService walletService, MessagePublisher messagePublisher)
        {
            _walletService = walletService;
            _messagePublisher = messagePublisher;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "walletQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<WalletUpdateMessage>(json);

                if (message != null)
                {
                    if (message.IsDeposit)
                    {
                        var response = _walletService.Deposit(message);

                        if (message.IsBet)
                        {
                            _messagePublisher.PublishMessage<WalletBalanceUpdateMessage>(response, "gameOutcomeQueue");
                        }
                        else
                        {
                            _messagePublisher.PublishMessage<WalletBalanceUpdateMessage>(response, "walletBalanceUpdateQueue");
                        }
                    }
                    else
                    {
                        var response = _walletService.Withdraw(message);

                        _messagePublisher.PublishMessage<WalletBalanceUpdateMessage>(response, "walletBalanceUpdateQueue");
                    }
                }
            };

            _channel.BasicConsume(queue: "walletQueue", autoAck: true, consumer: consumer);
        }

        public void StopListening()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
