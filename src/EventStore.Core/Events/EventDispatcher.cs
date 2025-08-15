using EventStore.ProjectionBuilders;

namespace EventStore.Events;

public class EventDispatcher(IServiceProvider serviceProvider, Lazy<IProjectionBuilderRegistration> registration)
{
    public async Task SendEventAsync<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent
    {
        var eventType = @event.GetType();
        var projectionBuilderTypes = registration.Value.ProjectionBuildersFor(eventType);

        foreach (var projectionBuilderType in projectionBuilderTypes)
        {
            var projectionBuilder = serviceProvider.GetService(projectionBuilderType)!;
            var applyEventsMethod = projectionBuilderType.GetMethod("ApplyEventAsync");

            var task = (Task)applyEventsMethod!.Invoke(projectionBuilder, [@event, token])!;
            await task.ConfigureAwait(false);
        }
    }
}
