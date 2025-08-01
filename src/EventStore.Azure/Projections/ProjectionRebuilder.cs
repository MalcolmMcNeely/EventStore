using EventStore.Events.Streams;
using EventStore.Projections;

namespace EventStore.Azure.Projections;

public class ProjectionRebuilder(ProjectionRebuilderRegistration registration, IEventStreamFactory eventStreamFactory)
{
    public Task<bool> CanRebuild(string key)
    {
        // query if metadata row exists
        
        return Task.FromResult(false);
    }

    public Task<T> Rebuild<T>(string key) where T : IProjection, new()
    {
        return Task.FromResult(new T());
    }
}