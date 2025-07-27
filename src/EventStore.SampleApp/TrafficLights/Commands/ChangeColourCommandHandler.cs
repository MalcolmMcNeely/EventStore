using EventStore.Commands;
using EventStore.Commands.AggregateRoots;
using EventStore.SampleApp.TrafficLights.AggregateRoots;

namespace EventStore.SampleApp.TrafficLights.Commands;

public class ChangeColourCommandHandler(IAggregateRootRepository<TrafficLight> _repository) : ICommandHandler<ChangeColour>
{
    public async Task HandleAsync(ChangeColour command, CancellationToken token = default)
    {
        await _repository.CreateUnitOfWork(nameof(TrafficLight))
            .PerformAsync(x => x.ChangeColourAsync(command))
            .CompleteAsync(token);
    }
}