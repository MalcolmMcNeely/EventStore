using EventStore.Events;

namespace EventStore.Commands.AggregateRoot;

public interface IAggregateRootRepository<T> where T : AggregateRoot
{
    public Task<T?> LoadAsync(string key, CancellationToken token = default);
    public Task<bool> SaveAsync(T aggregateRoot, string key, CancellationToken token = default);
    public Task SendEventsAsync(IEnumerable<IEvent> events, CancellationToken token = default);
}