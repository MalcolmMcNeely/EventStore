using EventStore.AggregateRoots;
using EventStore.Commands;

namespace EventStore.Testing.SimpleTestDomain;

public class TestCommandHandler(IAggregateRootRepository<TestAggregateRoot> repository) : ICommandHandler<TestCommand>
{
    public async Task HandleAsync(TestCommand command, CancellationToken token)
    {
        await repository.CreateUnitOfWork(nameof(TestAggregateRoot), command)
            .PerformAsync(x => x.OnCommand(command))
            .CompleteAsync(token);
    }
}