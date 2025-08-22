using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.Testing.TestDomains.Idempotency;

public class IdempotencyProjectionBuilder : ProjectionBuilder<IdempotencyProjection>
{
    public IdempotencyProjectionBuilder(IProjectionRepository<IdempotencyProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory) 
    {
        Handles<IdempotencyEvent>(x => x.Stream, OnEvent);
    }

    void OnEvent(IdempotencyEvent @event, IdempotencyProjection projection)
    {
        projection.Data = @event.Data;
    }
}