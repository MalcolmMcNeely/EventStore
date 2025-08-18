using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.Azure.Projections;

public class ProjectionRebuilder(IEventStreamFactory eventStreamFactory, IServiceProvider serviceProvider)
{
    public async Task<bool> CanRebuildAsync(string key, CancellationToken token)
    {
        var eventStream = eventStreamFactory.For($"projection-{key}");
        return await eventStream.ExistsAsync(token).ConfigureAwait(false);
    }

    public async Task<T> RebuildAsync<T>(string key, CancellationToken token) where T : IProjection, new()
    {
        var eventStream = eventStreamFactory.For($"projection-{key}");
        var events = eventStream.GetAllEventsAsync(token);
        var projectionBuilder = (ProjectionBuilder<T>)serviceProvider.GetService(typeof(ProjectionBuilder<T>))!;

        if (projectionBuilder is null)
        {
            throw new ProjectionRebuilderException($"Projection builder {typeof(T)} not found in service provider");
        }
        
        var projection = new T { Id = key };

        await foreach (var @event in events)
        {
            projectionBuilder.ApplyEventToProjection(projection, @event);
        }

        return projection;
    }
}