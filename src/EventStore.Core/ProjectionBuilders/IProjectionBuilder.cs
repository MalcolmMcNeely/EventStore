using EventStore.Events;

namespace EventStore.ProjectionBuilders;

public interface IProjectionBuilder
{
    Task ApplyEventAsync<TEvent>(TEvent @event, CancellationToken token) where TEvent : IEvent;
}