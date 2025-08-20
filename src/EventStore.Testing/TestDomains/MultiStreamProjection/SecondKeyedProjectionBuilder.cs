using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.Testing.TestDomains.MultiStreamProjection;

public class SecondKeyedProjectionBuilder : ProjectionBuilder<SecondKeyedProjection>
{
    public SecondKeyedProjectionBuilder(IProjectionRepository<SecondKeyedProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        Handles<MultiStreamProjectionEvent>(x => x.Id, OnEvent);
    }

    void OnEvent(MultiStreamProjectionEvent @event, SecondKeyedProjection projection)
    {
        projection.Data = @event.Data;
    }
}