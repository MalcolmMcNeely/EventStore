using EventStore.AggregateRoots;

namespace EventStore.Testing.TestDomains.Simple;

public class SimpleAggregateRoot : AggregateRoot
{
    public string? Data { get; set; }

    public SimpleAggregateRoot()
    {
        Handles<SimpleEvent>(OnTestEvent);
    }

    void OnTestEvent(SimpleEvent @event)
    {
        Data = @event.Data;
    }

    public Task OnCommand(SimpleCommand command)
    {
        Update(new SimpleEvent { Data = command.Data });

        return Task.CompletedTask;
    }
}