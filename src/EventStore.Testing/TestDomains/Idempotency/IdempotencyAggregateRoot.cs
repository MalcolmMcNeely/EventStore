using EventStore.AggregateRoots;

namespace EventStore.Testing.TestDomains.Idempotency;

public class IdempotencyAggregateRoot : AggregateRoot
{
    public bool Created { get; set; }
    public string Data { get; set; }

    public IdempotencyAggregateRoot()
    {
        Handles<IdempotencyEvent>(OnIdempotencyEvent);
    }

    void OnIdempotencyEvent(IdempotencyEvent @event)
    {
        Created = true;
        Data = @event.Data;
    }

    public Task OnCommand(IdempotencyCommand command)
    {
        if (!Created)
        {
            Update(new IdempotencyEvent
            {
                Stream = command.Stream,
                Data = command.Data,
            });
        }

        return Task.CompletedTask;
    }
}