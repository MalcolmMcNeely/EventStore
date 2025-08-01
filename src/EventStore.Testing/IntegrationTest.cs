using EventStore.Commands;
using EventStore.Commands.Dispatching;
using EventStore.Events;
using EventStore.Events.Transport;
using EventStore.Testing.Configuration;

namespace EventStore.Testing;

public abstract class IntegrationTest
{
    ICommandDispatcher _commandDispatcher;
    IEventBroadcaster  _eventBroadcaster;
    IEventPump _eventPump;
    IEventTransport _eventTransport;

    [OneTimeSetUp]
    public void DefaultConfiguration()
    {
        TestConfiguration
            .Configure()
            .WithInMemoryServices()
            .Build();
    }

    [SetUp]
    public void DefaultSetup()
    {
        _commandDispatcher = GetService<ICommandDispatcher>();
        _eventBroadcaster = GetService<IEventBroadcaster>();
        _eventPump = GetService<IEventPump>();
        _eventTransport = GetService<IEventTransport>();
    }

    protected T GetService<T>() where T : class
    {
        return TestConfiguration.Resolve<T>();
    }

    protected async Task DispatchCommandAsync(ICommand command)
    {
        await _commandDispatcher.DispatchAsync(command);
    }

    protected async Task SendEventAsync(IEvent @event)
    {
        await _eventTransport.WriteEventAsync(@event);
        await _eventPump.PublishEventsAsync();
        await _eventBroadcaster.BroadcastEventAsync();
    }
}
