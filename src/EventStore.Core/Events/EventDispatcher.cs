using EventStore.ProjectionBuilders;

namespace EventStore.Events;

public class EventDispatcher(IServiceProvider serviceProvider, ProjectionBuilderRegistration registration) : IEventDispatcher
{
    public async Task SendEventAsync<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent
    {
        var eventType = @event.GetType();
        var projections = registration.ProjectionsFor(eventType);

        foreach (var projection in projections)
        {
            var projectionBuilderType = typeof(ProjectionBuilder<>).MakeGenericType(projection);
            var projectionBuilder = (IProjectionBuilder)serviceProvider.GetService(projectionBuilderType)!;
            var applyEventsMethod = projectionBuilderType.GetMethod("ApplyEventAsync");
            var task = (Task)applyEventsMethod!.Invoke(projectionBuilder, [@event, token])!;
            await task.ConfigureAwait(false);
        }
    }
}
