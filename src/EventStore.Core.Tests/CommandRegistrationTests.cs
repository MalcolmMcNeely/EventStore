using System.Reflection;
using EventStore.Commands;

namespace EventStore.Core.Tests;

[TestFixture]
public class CommandRegistrationTests
{
    [SetUp]
    public void Setup()
    {
        CommandRegistration.FromAssembly(Assembly.GetExecutingAssembly());
    }

    [Test]
    public void it_can_resolve_a_command_handler()
    {
        Assert.That(CommandRegistration.ResolveHandler<CommandRegistrationTestCommand>(), Is.Not.Null);
    }
}

public class CommandRegistrationTestCommand : ICommand;

public class CommandRegistrationTestCommandHandler : ICommandHandler<CommandRegistrationTestCommand>
{
    public Task HandleAsync(CommandRegistrationTestCommand command)
    {
        throw new NotImplementedException();
    }
}