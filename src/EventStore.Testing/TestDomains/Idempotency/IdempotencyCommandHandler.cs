using EventStore.AggregateRoots;
using EventStore.Commands;

namespace EventStore.Testing.TestDomains.Idempotency;

public class IdempotencyCommandHandler(IAggregateRootRepository<IdempotencyAggregateRoot> repository) : ICommandHandler<IdempotencyCommand>
{
    public async Task HandleAsync(IdempotencyCommand command, CancellationToken token)
    {
        await repository.CreateUnitOfWork(command.Stream, command)
            .PerformAsync(x => x.OnCommand(command))
            .CompleteAsync(token);
    }
}