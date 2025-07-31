using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.Projections;

namespace EventStore.ProjectionBuilders;

public delegate void ProjectionBuilderEventHandler<in TEvent, in TProjection>(TEvent @event, TProjection projection) 
    where TEvent : IEvent
    where TProjection : IProjection;

public abstract class ProjectionBuilder<TProjection>(IProjectionRepository<TProjection> repository, IEventStreamFactory eventStreamFactory) where TProjection : IProjection, new()
{
    Dictionary<Type, Delegate> Handlers { get; } = new();
    string Key { get; set; } = string.Empty;

    protected void WithKey(string key)
    {
        Key = key;
    }

    protected void Handles<TEvent>(ProjectionBuilderEventHandler<TEvent, TProjection> eventHandler)
        where TEvent : IEvent
    {
        Handlers[typeof(TEvent)] = eventHandler;
    }

    public IEnumerable<Type> GetEventTypes()
    {
        return Handlers.Keys;
    }

    public async Task ApplyEventAsync(IEvent @event, CancellationToken token)
    {
        var key = Key; // need options for key building

        var projection = await repository.LoadAsync(key, token);

        if (projection is null)
        {
            projection = new() { Id = key };
        }

        InvokeHandler(@event.GetType(), @event, projection);

        var eventStream = eventStreamFactory.For($"projection-{Key}");
        await eventStream.PublishAsync(@event, token);

        await repository.SaveAsync(projection, token);
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