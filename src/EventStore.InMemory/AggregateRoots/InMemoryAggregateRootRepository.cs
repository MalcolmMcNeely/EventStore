using EventStore.Commands.AggregateRoots;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.InMemory.AggregateRoots;

public class InMemoryAggregateRootRepository<T>(IEventTransport transport) : IAggregateRootRepository<T>  where T : AggregateRoot, new()
{
    public List<IEvent> NewEvents { get; } = new();

    Dictionary<string, T> AggregateRoots { get; } = new();
    
    public Task<T?> LoadAsync(string key, CancellationToken token = default)
    {
        if (AggregateRoots.TryGetValue(key, out var aggregateRoot))
        {
            return Task.FromResult<T?>(aggregateRoot);
        }

        return Task.FromResult<T?>(null);
    }

    public Task<bool> SaveAsync(T aggregateRoot, string key, CancellationToken token = default)
    {
        AggregateRoots[key] = aggregateRoot;

        return Task.FromResult(true);
    }

    public async Task SendEventsAsync(IEnumerable<IEvent> events, CancellationToken token = default)
    {
        NewEvents.AddRange(events);

        foreach (var @event in events)
        {
            await transport.SendEventAsync(@event, token);
        }
    }
}