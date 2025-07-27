using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.InMemory.TrafficLights.Events;

namespace EventStore.SampleApp.InMemory.TrafficLights.Reports;

public class TrafficLightProjectionBuilder : ProjectionBuilder<TrafficLightProjection>
{
    public TrafficLightProjectionBuilder(IProjectionRepository<TrafficLightProjection> repository) : base(repository)
    {
        Handles<ColourChanged>(OnColourChanged);
    }

    void OnColourChanged(ColourChanged @event, TrafficLightProjection projection)
    {
        projection.Colour = @event.Colour;
    }
}