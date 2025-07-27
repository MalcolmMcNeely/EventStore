using EventStore.Commands.AggregateRoot;

namespace EventStore.SampleApp.TrafficLights.AggregateRoots;

public class TrafficLight : AggregateRoot
{
    public const string Key =  "TrafficLight";

    string _colour = Colours.Red;

    public Task ChangeColour(string commandColour)
    {
        _colour = commandColour;

        return Task.CompletedTask;
    }
}