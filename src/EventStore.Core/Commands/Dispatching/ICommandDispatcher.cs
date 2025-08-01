namespace EventStore.Commands.Dispatching;

public interface ICommandDispatcher
{
    public Task DispatchAsync<TCommand>(TCommand command, CancellationToken token = default) where TCommand : ICommand;
}