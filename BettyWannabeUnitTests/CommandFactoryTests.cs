using Moq;
using Xunit;
using BettyWannabe.Factory;
using BettyWannabe.Interface;
using SharedClasses;
using BettyWannabe.Commands;
using SharedClasses.Interface;

public class CommandFactoryTests
{
    private readonly Mock<IMessagePublisher> mockMessagePublisher;
    private readonly CommandFactory commandFactory;

    public CommandFactoryTests()
    {
        mockMessagePublisher = new Mock<IMessagePublisher>();
        commandFactory = new CommandFactory(mockMessagePublisher.Object);
    }

    [Theory]
    [InlineData("deposit 100", typeof(DepositCommand))]
    [InlineData("withdraw 50", typeof(WithdrawCommand))]
    [InlineData("bet 20", typeof(BetCommand))]
    public void Parse_ReturnsCorrectCommandType_ForValidInput(string input, Type expectedType)
    {
        // Act
        var command = commandFactory.Parse(input);

        // Assert
        Assert.IsType(expectedType, command);
    }

    [Theory]
    [InlineData("")]
    [InlineData("unknowncommand")]
    public void Parse_ThrowsArgumentException_ForInvalidInput(string input)
    {
        // Assert
        Assert.Throws<ArgumentException>(() => commandFactory.Parse(input));
    }
}
