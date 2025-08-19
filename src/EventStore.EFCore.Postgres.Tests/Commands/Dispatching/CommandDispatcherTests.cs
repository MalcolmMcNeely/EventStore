using EventStore.Commands;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.Utility;
using NUnit.Framework;

namespace EventStore.EFCore.Postgres.Tests.Commands.Dispatching;

public class CommandDispatcherTests : IntegrationTest
{
    static int _counter;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithEFCoreServices()
            .With<ICommandHandler<TestCommand>, TestCommandHandler>()
            .Build();
    }

    [Test]
    public async Task it_can_dispatch_a_command()
    {
        _counter = 0;

        await DispatchCommandAsync(new TestCommand());

        Assert.That(_counter, Is.EqualTo(1));
    }

    [Test]
    public async Task it_can_dispatch_many_command()
    {
        _counter = 0;

        await TestUtility.InvokeManyAsync(async () => await DispatchCommandAsync(new TestCommand()), 2000);

        Assert.That(_counter, Is.EqualTo(2000));
    }

    class TestCommand : ICommand
    {
        public string CausationId { get; set; }
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