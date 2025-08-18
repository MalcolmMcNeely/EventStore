using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.SampleApp.Domain.Accounts.Projections;

public class TotalBusinessAccountProjectionBuilder : ProjectionBuilder<TotalBusinessAccountProjection>
{
    public TotalBusinessAccountProjectionBuilder(IProjectionRepository<TotalBusinessAccountProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        
    }
}