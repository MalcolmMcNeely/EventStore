using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.Testing.TestDomains.SimpleTestDomain;

public class TestKeyedProjectionBuilder : ProjectionBuilder<TestProjection>
{
    public TestKeyedProjectionBuilder(IProjectionRepository<TestProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        Handles<TestEvent>(x => x.Id, OnEvent);
    }

    void OnEvent(TestEvent @event, TestProjection projection)
    {
        projection.Data = @event.Data;
    }
}