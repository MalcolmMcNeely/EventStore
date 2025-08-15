using EventStore.EFCore.Postgres.Events;

namespace EventStore.Blazor.EFCore.Postgres.Services.Events;

public interface IEventService
{
    Task<List<EventStreamEntity>> GetEventsSince(int index, CancellationToken token = default);
}