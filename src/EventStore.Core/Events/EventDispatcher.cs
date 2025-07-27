using EventStore.ProjectionBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Events;

public class EventDispatcher(IServiceProvider serviceProvider) : IEventDispatcher
{
    public void SendEvent<T>(T @event, CancellationToken token = default) where T : IEvent
    {
        //var handlerType = typeof(ProjectionBuilder<>).MakeGenericType(command.GetType());
        var handlers = serviceProvider.GetServices(typeof(ProjectionBuilder<>));
        
        // if (handler is null)
        // {
        //     throw new Exception($"No handler found for command {command.GetType()}");
        // }
        //
        // var handleMethod = handlerType.GetMethod("HandleAsync");
        // await (Task)handleMethod!.Invoke(handler, [command, token])!;
    }
}
