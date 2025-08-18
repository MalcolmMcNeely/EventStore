using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.Domain.TrafficLights.Events;

namespace EventStore.SampleApp.Domain.TrafficLights.Projections;

public class TrafficLightProjectionBuilder : ProjectionBuilder<TrafficLightProjection>
{
    public TrafficLightProjectionBuilder(IProjectionRepository<TrafficLightProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        WithDefaultKey(nameof(TrafficLightProjection));
        Handles<ColourChanged>(OnColourChanged);
    }

    void OnColourChanged(ColourChanged @event, TrafficLightProjection projection)
    {
        projection.Colour = @event.Colour;
    }
}