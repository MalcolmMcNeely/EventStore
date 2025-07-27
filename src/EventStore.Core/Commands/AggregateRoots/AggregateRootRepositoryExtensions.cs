using EventStore.Commands.Transactions;

namespace EventStore.Commands.AggregateRoots;

public static class AggregateRootRepositoryExtensions
{
    public static UnitOfWork<T> CreateUnitOfWork<T>(this IAggregateRootRepository<T> repository, string key) where T : AggregateRoot, new()
    {
        return new UnitOfWork<T>(key, repository);
    }
}