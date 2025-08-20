using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.Testing.TestDomains.Simple;

namespace EventStore.Testing.TestDomains.MultiStreamProjection;

public class SecondKeyedProjectionBuilder : ProjectionBuilder<SecondKeyedProjection>
{
    public SecondKeyedProjectionBuilder(IProjectionRepository<SecondKeyedProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        Handles<SimpleEvent>(x => x.Id, OnEvent);
    }

    void OnEvent(SimpleEvent @event, SecondKeyedProjection projection)
    {
        projection.Data = @event.Data;
    }
}