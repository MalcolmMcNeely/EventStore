using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.Azure.TrafficLights.Events;

namespace EventStore.SampleApp.Azure.TrafficLights.Projections;

public class TrafficLightProjectionBuilder : ProjectionBuilder<TrafficLightProjection>
{
    public TrafficLightProjectionBuilder(IProjectionRepository<TrafficLightProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        WithKey(nameof(TrafficLightProjection));
        Handles<ColourChanged>(OnColourChanged);
    }

    void OnColourChanged(ColourChanged @event, TrafficLightProjection projection)
    {
        projection.Colour = @event.Colour;
    }
}