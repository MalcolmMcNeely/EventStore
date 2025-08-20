using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.Testing.TestDomains.Simple;

namespace EventStore.Testing.TestDomains.MultiStreamProjection;

public class FirstKeyedProjectionBuilder : ProjectionBuilder<FirstKeyedProjection>
{
    public FirstKeyedProjectionBuilder(IProjectionRepository<FirstKeyedProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        Handles<SimpleEvent>(x => x.Id, OnEvent);
    }

    void OnEvent(SimpleEvent @event, FirstKeyedProjection projection)
    {
        projection.Data = @event.Data;
    }
}