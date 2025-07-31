using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.InMemory.TrafficLights.Events;

namespace EventStore.SampleApp.InMemory.TrafficLights.Projections;

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