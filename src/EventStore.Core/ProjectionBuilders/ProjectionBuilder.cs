using EventStore.Events;
using EventStore.Projections;

namespace EventStore.ProjectionBuilders;

public interface IProjectionBuilder
{
    public IEnumerable<Type> GetEventTypes();
}

public delegate void ProjectionBuilderEventHandler<in TEvent, in TProjection>(TEvent @event, TProjection projection) 
    where TEvent : IEvent
    where TProjection : IProjection;

public abstract class ProjectionBuilder<TProjection>(IProjectionRepository<TProjection> repository) : IProjectionBuilder where TProjection : IProjection
{
    Dictionary<Type, Delegate> Handlers { get; } = new();

    protected void Handles<TEvent>(ProjectionBuilderEventHandler<TEvent, TProjection> eventHandler)
        where TEvent : IEvent
    {
        Handlers[typeof(TEvent)] = eventHandler;
    }

    public IEnumerable<Type> GetEventTypes()
    {
        return Handlers.Keys;
    }
    
    internal void ApplyEvents(TProjection projection, params IEvent[] events)
    {
        foreach (var @event in events)
        {
            InvokeHandler(@event.GetType(), @event, projection);
        }
    }

    void InvokeHandler<TEvent>(Type type, TEvent @event, TProjection projection)
    {
        if (Handlers.TryGetValue(type, out var @delegate))
        {
            var invokeMethod = @delegate.GetType().GetMethod("Invoke");
            invokeMethod!.Invoke(@delegate, [@event, projection]);
        }
        else
        {
            throw new Exception($"No handler registered for event type {typeof(TEvent).Name}");
        }
    }
}