using EventStore.Commands.AggregateRoot;

namespace EventStore.Transaction;

public class UnitOfWork<T> where T : AggregateRoot, new()
{
    readonly string _key;
    readonly IEventSourcedEntityRepository<T> _eventSourcedEntityRepository;
    readonly List<Func<T, Task>> _actions = new();
    
    internal UnitOfWork(string key, IEventSourcedEntityRepository<T> eventSourcedEntityRepository)
    {
        _key = key;
        _eventSourcedEntityRepository = eventSourcedEntityRepository;
    }
    
    public UnitOfWork<T> PerformAsync(Func<T, Task> action)
    {
        _actions.Add(action);

        return this;
    }

    public async Task CompleteAsync()
    {
        var entity = _eventSourcedEntityRepository.Load(_key) ?? new T();
        
        foreach (var action in _actions)
        {
            await action(entity);
        }

        _eventSourcedEntityRepository.Save(entity, _key);
    }
}