namespace EventStore.Commands;

public interface ICommandDispatcher
{
    public Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
}