using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.Projections;

namespace EventStore.ProjectionBuilders;

public delegate void ProjectionBuilderEventHandler<in TEvent, in TProjection>(TEvent @event, TProjection projection) 
    where TEvent : IEvent
    where TProjection : IProjection;

public delegate string KeyBuilder<in TEvent>(TEvent @event)
    where TEvent : IEvent;

public abstract class ProjectionBuilder<TProjection>(IProjectionRepository<TProjection> repository, IEventStreamFactory eventStreamFactory) : IProjectionBuilder 
    where TProjection : IProjection, new()
{
    Dictionary<Type, Action<IEvent, TProjection>> Handlers { get; } = new();
    Dictionary<Type, Func<IEvent, string>> KeyBuilders { get; } = new();

    string? DefaultKey { get; set; }

    protected void WithDefaultKey(string key)
    {
        DefaultKey = key;
    }

    protected void Handles<TEvent>(ProjectionBuilderEventHandler<TEvent, TProjection> handler) where TEvent : IEvent
    {
        if (DefaultKey is null)
        {
            throw new ProjectionBuilderException($"{typeof(TProjection).Name} builder has no default key");
        }

        KeyBuilders[typeof(TEvent)] = KeyBuilderWrapper;
        Handlers[typeof(TEvent)] = HandlerWrapper;

        return;

        string KeyBuilderWrapper(IEvent x) => DefaultKey;
        void HandlerWrapper(IEvent x, TProjection y) => handler((TEvent)x, y);
    }

    protected void Handles<TEvent>(KeyBuilder<TEvent> keyBuilder, ProjectionBuilderEventHandler<TEvent, TProjection> handler) where TEvent : IEvent
    {
        if (DefaultKey is not null)
        {
            throw new ProjectionBuilderException($"{typeof(TProjection).Name} builder has no default key");
        }

        KeyBuilders[typeof(TEvent)] = KeyBuilderWrapper;
        Handlers[typeof(TEvent)] = HandlerWrapper;

        return;

        string KeyBuilderWrapper(IEvent x) => keyBuilder((TEvent)x);
        void HandlerWrapper(IEvent x, TProjection y) => handler((TEvent)x, y);
    }

    public IEnumerable<Type> GetEventTypes()
    {
        return Handlers.Keys;
    }

    public async Task ApplyEventAsync<TEvent>(TEvent @event, CancellationToken token) where TEvent : IEvent
    {
        var key = GetKeyFor(@event);
        var projection = await repository.LoadAsync(key, token).ConfigureAwait(false);

        InvokeHandlerFor(@event, projection);

        var eventStream = eventStreamFactory.For($"projection-{key}");
        await eventStream.PublishAsync(@event, token).ConfigureAwait(false);

        await repository.SaveAsync(projection, token).ConfigureAwait(false);
    }

    public void ApplyEventToProjection(TProjection projection, IEvent @event)
    {
        InvokeHandlerFor(@event, projection);
    }

    string GetKeyFor<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var eventType = @event.GetType();

        if (!KeyBuilders.TryGetValue(eventType, out var keyBuilder))
        {
            throw new ProjectionBuilderException($"{typeof(TProjection).Name} builder does not have key builder for event {eventType.Name}");
        }

        return keyBuilder(@event);
    }

    void InvokeHandlerFor<TEvent>(TEvent @event, TProjection projection) where TEvent : IEvent
    {
        var eventType = @event.GetType();

        if (!Handlers.TryGetValue(eventType, out var handler))
        {
            throw new ProjectionBuilderException($"{typeof(TProjection).Name} builder does not have handler for event {eventType.Name}");
        }

        handler(@event, projection);
    }
}