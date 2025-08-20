using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.Testing.TestDomains.Simple;

public class SimpleProjectionBuilder : ProjectionBuilder<SimpleProjection>
{
    public SimpleProjectionBuilder(IProjectionRepository<SimpleProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        WithDefaultKey(nameof(SimpleProjection));
        Handles<SimpleEvent>(OnEvent);
    }

    void OnEvent(SimpleEvent @event, SimpleProjection projection)
    {
        projection.Data = @event.Data;
    }
}