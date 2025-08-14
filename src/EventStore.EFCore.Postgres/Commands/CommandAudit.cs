using EventStore.Commands;
using EventStore.Commands.Dispatching;

namespace EventStore.EFCore.Postgres.Commands;

public class CommandAudit : ICommandAudit
{
    public Task PublishAsync<T>(T command, CancellationToken token) where T : ICommand
    {
        throw new NotImplementedException();
    }
}