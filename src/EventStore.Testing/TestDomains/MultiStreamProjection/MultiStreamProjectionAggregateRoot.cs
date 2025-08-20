using EventStore.AggregateRoots;

namespace EventStore.Testing.TestDomains.MultiStreamProjection;

public class MultiStreamProjectionAggregateRoot : AggregateRoot
{
    public string? Data { get; set; }

    public MultiStreamProjectionAggregateRoot()
    {
        Handles<MultiStreamProjectionEvent>(OnTestEvent);
    }

    void OnTestEvent(MultiStreamProjectionEvent @event)
    {
        Data = @event.Data;
    }

    public Task OnCommand(MultiStreamProjectionCommand command)
    {
        Update(new MultiStreamProjectionEvent { Data = command.Data });

        return Task.CompletedTask;
    }
}