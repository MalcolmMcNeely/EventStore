using EventStore.AggregateRoots;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.InMemory.AggregateRoots;

public class AggregateRootRepository<TAggregateRoot>(IEventTransport transport) : IAggregateRootRepository<TAggregateRoot>  where TAggregateRoot : AggregateRoot, new()
{
    public List<IEvent> NewEvents { get; } = new();

    Dictionary<string, TAggregateRoot> AggregateRoots { get; } = new();
    
    public Task<TAggregateRoot> LoadAsync(string key, CancellationToken token = default)
    {
        if (AggregateRoots.TryGetValue(key, out var aggregateRoot))
        {
            return Task.FromResult(aggregateRoot);
        }

        return Task.FromResult(new TAggregateRoot {  Id = key });
    }

    public Task<bool> SaveAsync(TAggregateRoot aggregateRoot, string key, CancellationToken token = default)
    {
        AggregateRoots[key] = aggregateRoot;

        return Task.FromResult(true);
    }

    public async Task SendEventAsync<TEvent>(TEvent @event, string key, CancellationToken token = default) where TEvent : class, IEvent
    {
        NewEvents.Add(@event);

        await transport.WriteEventAsync(@event, token);
    }
}