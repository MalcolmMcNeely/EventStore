using EventStore.Commands;
using EventStore.Commands.AggregateRoots;
using EventStore.SampleApp.Domain.TrafficLights.AggregateRoots;

namespace EventStore.SampleApp.Domain.TrafficLights.Commands;

public class ChangeColourCommandHandler(IAggregateRootRepository<TrafficLight> repository) : ICommandHandler<ChangeColour>
{
    public async Task HandleAsync(ChangeColour command, CancellationToken token = default)
    {
        await repository.CreateUnitOfWork(nameof(TrafficLight), command)
            .PerformAsync(x => x.ChangeColourAsync(command))
            .CompleteAsync(token);
    }
}