using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.Testing.TestDomains.SimpleTestDomain;

public class TestDefaultKeyProjectionBuilder : ProjectionBuilder<TestProjection>
{
    public TestDefaultKeyProjectionBuilder(IProjectionRepository<TestProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        WithDefaultKey(nameof(TestProjection));
        Handles<TestEvent>(OnEvent);
    }

    void OnEvent(TestEvent @event, TestProjection projection)
    {
        projection.Data = @event.Data;
    }
}