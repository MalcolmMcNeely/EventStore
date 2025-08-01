namespace EventStore.Commands.Dispatching;

public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public async Task DispatchAsync<T>(T command, CancellationToken token) where T : ICommand
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = serviceProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new CommandDispatcherException($"No handler found for command {command.GetType().Name}");
        }

        var handleMethod = handlerType.GetMethod("HandleAsync");
        await (Task)handleMethod!.Invoke(handler, [command, token])!;
    }
}