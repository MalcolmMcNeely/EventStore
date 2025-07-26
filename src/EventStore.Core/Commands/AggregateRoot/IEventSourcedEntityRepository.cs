namespace EventStore.Commands.AggregateRoot;

public interface IEventSourcedEntityRepository<T> where T : AggregateRoot
{
    public T? Load(string key);
    public void Save(T aggregateRoot, string key);
}