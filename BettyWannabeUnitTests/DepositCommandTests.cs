using Moq;
using System.Threading.Tasks;
using Xunit;
using BettyWannabe.Interface;
using BettyWannabe.Commands;
using SharedClasses;
using SharedClasses.Interface;

public class DepositCommandTests
{
    [Fact]
    public async Task ExecuteAsync_CallsPublishMessageAsync_WithCorrectParameters()
    {
        // Arrange
        var mockMessagePublisher = new Mock<IMessagePublisher>();
        var amount = 100m;
        var depositCommand = new DepositCommand(amount, mockMessagePublisher.Object);

        // Act
        await depositCommand.ExecuteAsync();

        // Assert
        mockMessagePublisher.Verify(
            x => x.PublishMessageAsync(It.Is<WalletUpdateMessage>(m => m.Amount == amount && m.IsDeposit && !m.IsBet), "walletQueue"),
            Times.Once);
    }
}
