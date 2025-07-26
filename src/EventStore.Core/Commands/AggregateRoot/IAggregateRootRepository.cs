namespace EventStore.Commands.AggregateRoot;

public interface IAggregateRootRepository<T> where T : AggregateRoot
{
    public Task<T?> LoadAsync(string key);
    public Task<bool> SaveAsync(T aggregateRoot, string key);
}