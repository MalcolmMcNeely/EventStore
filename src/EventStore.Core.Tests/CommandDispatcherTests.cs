using EventStore.Commands;
using Moq;

namespace EventStore.Core.Tests;

[TestFixture]
public class CommandDispatcherTests
{
    Mock<IServiceProvider> _mockServiceProvider;
    Mock<ICommandHandler<CommandDispatcherTestCommand>> _mockCommandHandler;
    CommandDispatcher _commandDispatcher;

    [SetUp]
    public void SetUp()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockCommandHandler = new Mock<ICommandHandler<CommandDispatcherTestCommand>>();

        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(ICommandHandler<CommandDispatcherTestCommand>)))
            .Returns(_mockCommandHandler.Object);
        _mockCommandHandler
            .Setup(x => x.HandleAsync(It.IsAny<CommandDispatcherTestCommand>()))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.AtLeast(3));

        _commandDispatcher = new CommandDispatcher(_mockServiceProvider.Object);
    }

    [Test]
    public async Task it_can_dispatch_a_command()
    {
        await _commandDispatcher.DispatchAsync(new CommandDispatcherTestCommand());
        _mockCommandHandler.Verify(x => x.HandleAsync(It.IsAny<CommandDispatcherTestCommand>()), Times.Once);
    }
}

public class CommandDispatcherTestCommand : ICommand;