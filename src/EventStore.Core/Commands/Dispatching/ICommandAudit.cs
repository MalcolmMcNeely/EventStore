namespace EventStore.Commands.Dispatching;

public interface ICommandAudit
{
    Task PublishAsync<T>(T command, CancellationToken token) where T : ICommand;
}