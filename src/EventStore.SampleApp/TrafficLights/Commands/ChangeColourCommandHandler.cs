using EventStore.Commands;
using EventStore.Commands.AggregateRoot;
using EventStore.SampleApp.TrafficLights.AggregateRoot;

namespace EventStore.SampleApp.TrafficLights.Commands;

public class ChangeColourCommandHandler(IEventSourcedEntityRepository<TrafficLight> _repository) : ICommandHandler<ChangeColour>
{
    public async Task HandleAsync(ChangeColour command)
    {
        await _repository.CreateUnitOfWork(TrafficLight.Key)
            .PerformAsync(x => x.ChangeColour(command.Colour))
            .CompleteAsync();
    }
}