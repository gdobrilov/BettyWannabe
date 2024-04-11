using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using BettyWannabe.Commands;
using BettyWannabe.Interface;
using SharedClasses;
using SharedClasses.Interface;

namespace BettyWannabe.Tests
{
    public class BetCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_ValidAmount_PublishesMessage()
        {
            // Arrange
            var mockPublisher = new Mock<IMessagePublisher>();
            var betCommand = new BetCommand(5m, mockPublisher.Object);

            // Act
            await betCommand.ExecuteAsync();

            // Assert
            mockPublisher.Verify(p => p.PublishMessageAsync(It.IsAny<WalletUpdateMessage>(), "walletQueue"), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public async Task ExecuteAsync_InvalidAmount_ThrowsArgumentOutOfRangeException(decimal amount)
        {
            // Arrange
            var mockPublisher = new Mock<IMessagePublisher>();
            var betCommand = new BetCommand(amount, mockPublisher.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => betCommand.ExecuteAsync());
        }
    }
}
