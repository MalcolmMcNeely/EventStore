using EventStore.AggregateRoots;
using EventStore.Commands;

namespace EventStore.Testing.TestDomains.MultiStreamProjection;

public class MultiStreamProjectionCommandHandler(IAggregateRootRepository<MultiStreamProjectionAggregateRoot> repository) : ICommandHandler<MultiStreamProjectionCommand>
{
    public async Task HandleAsync(MultiStreamProjectionCommand command, CancellationToken token)
    {
        await repository.CreateUnitOfWork(nameof(MultiStreamProjectionAggregateRoot), command)
            .PerformAsync(x => x.OnCommand(command))
            .CompleteAsync(token);
    }
}

