using EventStore.Commands.AggregateRoots;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.EFCore.Postgres.AggregateRoots;

public class AggregateRootRepository<TAggregateRoot>(IEventTransport transport) : IAggregateRootRepository<TAggregateRoot>  where TAggregateRoot : AggregateRoot, new()
{
    public Task<TAggregateRoot> LoadAsync(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveAsync(TAggregateRoot aggregateRoot, string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task SendEventAsync<TEvent>(TEvent @event, string key, CancellationToken token = default) where TEvent : class, IEvent
    {
        throw new NotImplementedException();
    }
}