using EventStore.Commands;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.Utility;

namespace EventStore.Core.Tests.Commands.Dispatching;

public class CommandDispatcherTests : IntegrationTest
{
    static int _counter;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .With<ICommandHandler<TestCommand>, TestCommandHandler>()
            .Build();
    }

    [Test]
    public async Task it_can_dispatch_a_command()
    {
        _counter = 0;
        
        await DispatchCommandAsync(new TestCommand());

        await Verify(_counter);
    }
    
    [Test]
    public async Task it_can_dispatch_many_commands()
    {
        _counter = 0;

        await TestUtility.InvokeManyAsync(async () => await DispatchCommandAsync(new TestCommand()), 2000);

        await Verify(_counter);
    }
    
    class TestCommand : ICommand
    {
        public string CausationId { get; set; } = Guid.NewGuid().ToString();
    }

    class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public Task HandleAsync(TestCommand command, CancellationToken token)
        {
            _counter++;
            
            return Task.CompletedTask;
        }
    }
}

