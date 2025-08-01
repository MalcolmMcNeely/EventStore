using EventStore.Commands;
using EventStore.Commands.AggregateRoots;

namespace EventStore.Testing.BasicTestCase;

public class TestCommandHandler(IAggregateRootRepository<TestAggregateRoot> repository) : ICommandHandler<TestCommand>
{
    public async Task HandleAsync(TestCommand command, CancellationToken token)
    {
        await repository.CreateUnitOfWork(nameof(TestAggregateRoot))
            .PerformAsync(x => x.OnCommand(command))
            .CompleteAsync(token);
    }
}