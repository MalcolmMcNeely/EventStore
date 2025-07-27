namespace EventStore.Commands;

public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public async Task DispatchAsync<T>(T command, CancellationToken token = default) where T : ICommand
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = serviceProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new Exception($"No handler found for command {command.GetType()}");
        }

        var handleMethod = handlerType.GetMethod("HandleAsync");
        await (Task)handleMethod!.Invoke(handler, [command, token])!;
    }
}