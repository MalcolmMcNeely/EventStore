using EventStore.Transaction;

namespace EventStore.Commands.AggregateRoot;

public static class AggregateRootRepositoryExtensions
{
    public static UnitOfWork<T> CreateUnitOfWork<T>(this IAggregateRootRepository<T> repository, string key) where T : AggregateRoot, new()
    {
        return new UnitOfWork<T>(key, repository);
    }
}