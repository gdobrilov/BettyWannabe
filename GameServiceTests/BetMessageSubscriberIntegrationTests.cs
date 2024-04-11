using System.Text;
using System.Text.Json;
using GameService.Interface;
using GameService.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using RabbitMQ.Client;
using SharedClasses;
using SharedClasses.Interface;

public class BetMessageSubscriberIntegrationTests : IDisposable
{
    private IConnection connection;
    private IModel channel;
    private Mock<IGameService> mockGameService;
    private Mock<IMessagePublisher> mockMessagePublisher;
    private BetMessageSubscriber subscriber;

    public BetMessageSubscriberIntegrationTests()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.QueueDeclare("betQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        mockGameService = new Mock<IGameService>();
        mockMessagePublisher = new Mock<IMessagePublisher>();

        subscriber = new BetMessageSubscriber(mockGameService.Object, mockMessagePublisher.Object);
    }

    [Fact]
    public async Task Subscriber_ProcessesMessages_Correctly()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddHostedService(_ => subscriber);
            })
            .Build();

        await host.StartAsync();

        var betMessage = new BetMessage { Amount = 100, CurrentBalance = 500 };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(betMessage));
        channel.BasicPublish("", "betQueue", null, body);

        await Task.Delay(1000);

        mockGameService.Verify(g => g.PlayBetAsync(It.IsAny<BetMessage>()), Times.Once);
        mockMessagePublisher.VerifyNoOtherCalls();

        await host.StopAsync();
    }

    public void Dispose()
    {
        channel?.Dispose();
        connection?.Dispose();
    }
}
