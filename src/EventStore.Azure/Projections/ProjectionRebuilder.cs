using EventStore.Events.Streams;
using EventStore.Projections;

namespace EventStore.Azure.Projections;

public class ProjectionRebuilder(ProjectionRebuilderRegistration registration, IEventStreamFactory eventStreamFactory)
{
    public async Task<bool> CanRebuildAsync(string key, CancellationToken token)
    {
        var eventStream = eventStreamFactory.For($"projection-{key}");
        return await eventStream.ExistsAsync(token);
    }

    public async Task<T> RebuildAsync<T>(string key, CancellationToken token) where T : IProjection, new()
    {
        var eventStream = eventStreamFactory.For($"projection-{key}");
        var events = eventStream.GetAllEventsAsync(token);

        var projectionBuilder = registration.ProjectionBuilderFor<T>();
        var projection = new T { Id = key };

        await foreach (var @event in events)
        {
            projectionBuilder.ApplyEventToProjection(projection, @event, token);
        }

        return projection;
    }
}