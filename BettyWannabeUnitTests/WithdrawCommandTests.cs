using Moq;
using System.Threading.Tasks;
using Xunit;
using BettyWannabe.Interface;
using BettyWannabe.Commands;
using SharedClasses;
using SharedClasses.Interface;

public class WithdrawCommandTests
{
    [Fact]
    public async Task ExecuteAsync_CallsPublishMessageAsync_WithCorrectParameters()
    {
        // Arrange
        var mockMessagePublisher = new Mock<IMessagePublisher>();
        decimal amount = 100m;
        var withdrawCommand = new WithdrawCommand(amount, mockMessagePublisher.Object);

        // Act
        await withdrawCommand.ExecuteAsync();

        // Assert
        mockMessagePublisher.Verify(
            x => x.PublishMessageAsync(It.Is<WalletUpdateMessage>(m => m.Amount == amount && !m.IsDeposit && !m.IsBet), "walletQueue"),
            Times.Once,
            "PublishMessageAsync should be called once with a WalletUpdateMessage where IsDeposit and IsBet are false.");
    }
}
