namespace EventStore.EFCore.Postgres.Events.Cursors;

public class EventCursorFactory
{
    internal async Task<EventCursor> GetOrAddCursorAsync(string cursorName, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    internal async Task SaveCursorAsync(EventCursor eventCursorEntity, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}