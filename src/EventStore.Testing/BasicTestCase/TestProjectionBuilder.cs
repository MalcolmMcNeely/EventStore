using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.Testing.BasicTestCase;

public class TestProjectionBuilder : ProjectionBuilder<TestProjection>
{
    public TestProjectionBuilder(IProjectionRepository<TestProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        WithKey(nameof(TestProjection));
        Handles<TestEvent>(OnEvent);
    }

    void OnEvent(TestEvent @event, TestProjection projection)
    {
        projection.Data = @event.Data;
    }
}