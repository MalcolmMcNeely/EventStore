using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.Testing.TestDomains.MultiStreamProjection;

public class FirstKeyedProjectionBuilder : ProjectionBuilder<FirstKeyedProjection>
{
    public FirstKeyedProjectionBuilder(IProjectionRepository<FirstKeyedProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        Handles<MultiStreamProjectionEvent>(x => x.Id, OnEvent);
    }

    void OnEvent(MultiStreamProjectionEvent @event, FirstKeyedProjection projection)
    {
        projection.Data = @event.Data;
    }
}