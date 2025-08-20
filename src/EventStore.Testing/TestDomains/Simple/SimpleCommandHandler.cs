using EventStore.AggregateRoots;
using EventStore.Commands;

namespace EventStore.Testing.TestDomains.Simple;

public class SimpleCommandHandler(IAggregateRootRepository<SimpleAggregateRoot> repository) : ICommandHandler<SimpleCommand>
{
    public async Task HandleAsync(SimpleCommand command, CancellationToken token)
    {
        await repository.CreateUnitOfWork(nameof(SimpleAggregateRoot), command)
            .PerformAsync(x => x.OnCommand(command))
            .CompleteAsync(token);
    }
}