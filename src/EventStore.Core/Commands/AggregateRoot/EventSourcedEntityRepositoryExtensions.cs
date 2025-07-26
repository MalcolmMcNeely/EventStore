using EventStore.Transaction;

namespace EventStore.Commands.AggregateRoot;

public static class EventSourcedEntityRepositoryExtensions
{
    public static UnitOfWork<T> CreateUnitOfWork<T>(this IEventSourcedEntityRepository<T> repository, string key) where T : AggregateRoot, new()
    {
        return new UnitOfWork<T>(key, repository);
    }
}