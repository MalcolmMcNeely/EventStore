using EventStore.Events;

namespace EventStore.Commands.AggregateRoots;

public interface IAggregateRootRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot, new()
{
    public Task<TAggregateRoot?> LoadAsync(string key, CancellationToken token = default);
    public Task<bool> SaveAsync(TAggregateRoot aggregateRoot, string key, CancellationToken token = default);
    public Task SendEventAsync<TEvent>(TEvent @event, string key, CancellationToken token = default) where TEvent : class, IEvent;
}