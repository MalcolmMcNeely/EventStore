using EventStore.Commands.AggregateRoots;
using EventStore.Events;

namespace EventStore.Core.Tests.Commands.Transactions;

public class InMemoryAggregateRootRepository<T> : IAggregateRootRepository<T>  where T : AggregateRoot, new()
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
        AggregateRoots.Add(key, aggregateRoot);

        return Task.FromResult(true);
    }

    public Task SendEventsAsync(IEnumerable<IEvent> events, CancellationToken token = default)
    {
        NewEvents.AddRange(events);

        return Task.CompletedTask;
    }
}