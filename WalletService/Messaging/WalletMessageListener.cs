using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace WalletService.Messaging
{
    public class WalletMessageListener : BackgroundService
    {
        private readonly MessageSubscriber _messageSubscriber;

        public WalletMessageListener(MessageSubscriber subscriber)
        {
            _messageSubscriber = subscriber;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageSubscriber.StartListening();
            stoppingToken.Register(() => _messageSubscriber.StopListening());

            return Task.CompletedTask;
        }
    }
}
