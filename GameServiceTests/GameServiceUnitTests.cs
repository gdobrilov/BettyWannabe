using System.Threading.Tasks;
using Moq;
using Xunit;
using GameService.Models;
using GameService.Interface;
using SharedClasses;
using GameService.Messaging;
using GameService.Services;
using SharedClasses.Interface;


public class GameServiceUnitTests
{
    private readonly Mock<IGame> mockGame;
    private readonly Mock<IMessagePublisher> mockPublisher;
    private readonly GameService.Services.GameService service;

    public GameServiceUnitTests()
    {
        mockGame = new Mock<IGame>();
        mockPublisher = new Mock<IMessagePublisher>();
        service = new GameService.Services.GameService(mockGame.Object, mockPublisher.Object);
    }
    [Fact]
    public async Task PlayBetAsync_WinningOutcome_PublishesCorrectMessage()
    {
        // Arrange
        var betMessage = new BetMessage { Amount = 5, CurrentBalance = 10 };
        mockGame.Setup(g => g.PlayGame(It.IsAny<Bet>())).Returns((true, 6.52m));

        // Act
        var result = await service.PlayBetAsync(betMessage);

        // Assert
        mockPublisher.Verify(p => p.PublishMessageAsync(It.Is<WalletUpdateMessage>(
            m => m.Amount == 6.52m && m.IsDeposit && m.IsBet), "walletQueue"), Times.Once);
        Assert.True(result.IsDeposit);
        Assert.Equal(6.52m, result.Amount);
    }

    [Fact]
    public async Task PlayBetAsync_LosingOutcome_PublishesCorrectMessage()
    {
        // Arrange
        var betMessage = new BetMessage { Amount = 5, CurrentBalance = 10 };
        mockGame.Setup(g => g.PlayGame(It.IsAny<Bet>())).Returns((false, 0m));

        // Act
        var result = await service.PlayBetAsync(betMessage);

        // Assert
        mockPublisher.Verify(p => p.PublishMessageAsync(It.Is<WalletBalanceUpdateMessage>(
            m => m.CurrentBalance == 10 && m.IsSuccessful), "gameOutcomeQueue"), Times.Once);
        Assert.False(result.IsDeposit);
        Assert.Equal(0, result.Amount);
    }
}
