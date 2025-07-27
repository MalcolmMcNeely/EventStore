using EventStore.Events;

namespace EventStore.Commands.AggregateRoots;

public delegate void AggregateRootEventHandler<in T>(T @event) where T : IEvent;

public abstract class AggregateRoot
{
    public List<(Type Type, IEvent @event)> NewEvents { get; } = new();

    Dictionary<Type, Delegate> Handlers { get; } = new();

    protected void Handles<T>(AggregateRootEventHandler<T> eventHandler) where T : IEvent
    {
        Handlers[typeof(T)] = eventHandler;
    }
    
    protected void Update<T>(T @event) where T : IEvent
    {
        NewEvents.Add((typeof(T), @event));
    }

    internal void ApplyUpdates()
    {
        foreach (var newEvents in NewEvents)
        {
            InvokeHandler(newEvents.Type, newEvents.@event);
        }
    }

    void InvokeHandler<T>(Type type, T @event) where T : IEvent
    {
        if (Handlers.TryGetValue(type, out var @delegate))
        {
            var invokeMethod = @delegate.GetType().GetMethod("Invoke");
            invokeMethod!.Invoke(@delegate, [@event]);
        }
        else
        {
            throw new Exception($"No handler registered for event type {typeof(T).Name}");
        }
    }
}