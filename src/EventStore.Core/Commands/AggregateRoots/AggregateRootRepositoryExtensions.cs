using EventStore.Commands.Transactions;

namespace EventStore.Commands.AggregateRoots;

public static class AggregateRootRepositoryExtensions
{
    public static UnitOfWork<T> CreateUnitOfWork<T>(this IAggregateRootRepository<T> repository, string key, ICommand command) where T : AggregateRoot, new()
    {
        return new UnitOfWork<T>(key, repository, command.CausationId);
    }
}