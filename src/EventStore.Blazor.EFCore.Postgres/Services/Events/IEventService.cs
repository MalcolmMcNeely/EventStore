using EventStore.EFCore.Postgres.Events;

namespace EventStore.Blazor.EFCore.Postgres.Services.Events;

public interface IEventService
{
    Task<(bool HasNewEvents, List<EventStreamEntity> Events)> GetEventsSince(DateTime time, CancellationToken token = default);
}