using EventStore.Commands;
using EventStore.Commands.Dispatching;
using EventStore.Events;
using EventStore.Events.Transport;
using EventStore.Testing.Configuration;

namespace EventStore.Testing;

public abstract class IntegrationTest
{
    ICommandDispatcher _commandDispatcher;
    IEventBroadcaster _eventBroadcaster;
    IEventPump _eventPump;
    IEventTransport _eventTransport;

    [SetUp]
    public void IntegrationTestSetup()
    {
        Configure();

        _commandDispatcher = GetService<ICommandDispatcher>()!;
        _eventBroadcaster = GetService<IEventBroadcaster>()!;
        _eventPump = GetService<IEventPump>()!;
        _eventTransport = GetService<IEventTransport>()!;
    }

    protected abstract void Configure();

    protected static T GetService<T>() where T : class => TestConfiguration.Resolve<T>();
    protected static T GetScopedService<T>() where T : class => TestConfiguration.ResolveScoped<T>();

    protected async Task DispatchCommandAsync(ICommand command)
    {
        await _commandDispatcher.DispatchAsync(command);
        await _eventPump.PublishEventsAsync();
        await _eventBroadcaster.BroadcastEventAsync();
    }

    protected async Task SendEventAsync(IEvent @event)
    {
        await _eventTransport.WriteEventAsync(@event);
        await _eventPump.PublishEventsAsync();
        await _eventBroadcaster.BroadcastEventAsync();
    }
}