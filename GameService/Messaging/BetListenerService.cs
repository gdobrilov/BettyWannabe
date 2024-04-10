using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace GameService.Messaging
{
    public class BetListenerService : BackgroundService
    {
        private readonly BetMessageSubscriber _subscriber;

        public BetListenerService(BetMessageSubscriber subscriber)
        {
            _subscriber = subscriber;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _subscriber.StartListening();
            stoppingToken.Register(() => _subscriber.StopListening());

            return Task.CompletedTask;
        }
    }
}
