using EventStore.Events;

namespace EventStore.SampleApp.Domain.Accounts.Events;

public class Event : IEvent
{
    public required string User { get; set; }
    public string CausationId { get; set; }
}