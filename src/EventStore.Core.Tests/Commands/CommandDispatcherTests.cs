using EventStore.Commands;
using EventStore.Commands.Dispatching;
using Moq;

namespace EventStore.Core.Tests.Commands;

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
            .Setup(x => x.HandleAsync(It.IsAny<CommandDispatcherTestCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _commandDispatcher = new CommandDispatcher(_mockServiceProvider.Object);
    }

    [Test]
    public async Task it_can_dispatch_a_command()
    {
        await _commandDispatcher.DispatchAsync(new CommandDispatcherTestCommand());
        _mockCommandHandler.Verify(x => x.HandleAsync(It.IsAny<CommandDispatcherTestCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    public class CommandDispatcherTestCommand : ICommand;
}

