using EventStore.Commands;
using EventStore.Commands.AggregateRoot;
using EventStore.SampleApp.TrafficLights.AggregateRoots;

namespace EventStore.SampleApp.TrafficLights.Commands;

public class ChangeColourCommandHandler(IAggregateRootRepository<TrafficLight> _repository) : ICommandHandler<ChangeColour>
{
    public async Task HandleAsync(ChangeColour command)
    {
        await _repository.CreateUnitOfWork(TrafficLight.Key)
            .PerformAsync(x => x.ChangeColour(command.Colour))
            .CompleteAsync();
    }
}