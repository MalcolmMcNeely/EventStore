using EventStore.Events;

namespace EventStore.Commands.AggregateRoots;

public interface IAggregateRootRepository<T> where T : AggregateRoot, new()
{
    public Task<T?> LoadAsync(string key, CancellationToken token = default);
    public Task<bool> SaveAsync(T aggregateRoot, string key, CancellationToken token = default);
    public Task SendEventsAsync(IEnumerable<IEvent> events, CancellationToken token = default);
}