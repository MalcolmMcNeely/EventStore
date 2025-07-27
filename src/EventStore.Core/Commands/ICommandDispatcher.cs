namespace EventStore.Commands;

public interface ICommandDispatcher
{
    public Task DispatchAsync<TCommand>(TCommand command, CancellationToken token) where TCommand : ICommand;
}