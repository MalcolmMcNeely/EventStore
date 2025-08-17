using EventStore.Commands;
using EventStore.Commands.Dispatching;

namespace EventStore.InMemory.Commands;

public class CommandAudit : ICommandAudit
{
    public Task PublishAsync<T>(T command, CancellationToken token) where T : ICommand
    {
        return Task.CompletedTask;
    }
}