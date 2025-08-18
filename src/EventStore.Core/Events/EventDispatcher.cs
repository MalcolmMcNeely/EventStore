using EventStore.ProjectionBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Events;

public class EventDispatcher(IServiceScopeFactory scopeFactory, Lazy<IProjectionBuilderRegistration> registration)
{
    public async Task SendEventAsync<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent
    {
        var eventType = @event.GetType();
        var projectionBuilderTypes = registration.Value.ProjectionBuildersFor(eventType);

        using var scope = scopeFactory.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        foreach (var projectionBuilderType in projectionBuilderTypes)
        {
            var projectionBuilder = (IProjectionBuilder)scopedProvider.GetRequiredService(projectionBuilderType)!;
            await projectionBuilder.ApplyEventAsync(@event, token).ConfigureAwait(false);
        }
    }
}
