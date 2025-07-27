namespace EventStore.Commands;

public interface ICommandHandler<T> where T : ICommand
{
    public Task HandleAsync(T command, CancellationToken token);
}