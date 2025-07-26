namespace EventStore.SampleApp.TrafficLights.AggregateRoot;

public class TrafficLight : EventStore.Commands.AggregateRoot.AggregateRoot
{
    public const string Key =  "TrafficLight";

    string _colour = Colours.Red;

    public Task ChangeColour(string commandColour)
    {
        _colour = commandColour;

        return Task.CompletedTask;
    }
}