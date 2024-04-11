using BettyWannabe;
using BettyWannabe.Interface;
using Moq;

public class CommandHandlerTests
{
    [Fact]
    public async Task ExecutesCommandFromInputSuccessfully()
    {
        // Arrange
        var mockConsoleService = new Mock<IConsoleService>();
        var mockCommandFactory = new Mock<ICommandFactory>();
        var mockMessageResponseService = new Mock<IMessageResponseService>();
        var mockCommand = new Mock<ICommand>();

        mockConsoleService.SetupSequence(x => x.ReadLine())
            .Returns("test command")
            .Returns("exit");

        mockCommand.Setup(m => m.ExecuteAsync()).Returns(Task.CompletedTask);
        mockCommandFactory.Setup(x => x.Parse("test command")).Returns(mockCommand.Object);

        var handler = new CommandHandler(mockCommandFactory.Object, mockMessageResponseService.Object, mockConsoleService.Object);

        // Act
        var cancellationTokenSource = new CancellationTokenSource();
        var runningTask = Task.Run(() => handler.StartAsync(cancellationTokenSource.Token));

        await Task.Delay(1500);

        cancellationTokenSource.Cancel();
        await runningTask;

        // Assert
        mockCommand.Verify(x => x.ExecuteAsync(), Times.Once);
        mockConsoleService.Verify(x => x.Write("Please submit action: "), Times.AtLeastOnce);
    }

}
