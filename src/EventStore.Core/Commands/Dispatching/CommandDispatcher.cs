using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Commands.Dispatching;

public class CommandDispatcher(IServiceScopeFactory scopeFactory, ICommandAudit commandAudit) : ICommandDispatcher
{
    public async Task DispatchAsync<T>(T command, CancellationToken token) where T : ICommand
    {
        using var scope = scopeFactory.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = scopedProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new CommandDispatcherException($"No handler found for command {command.GetType().Name}");
        }

        var handleMethod = handlerType.GetMethod("HandleAsync");
        await ((Task)handleMethod!.Invoke(handler, [command, token])!).ConfigureAwait(false);
        await commandAudit.PublishAsync(command, token).ConfigureAwait(false);
    }
}