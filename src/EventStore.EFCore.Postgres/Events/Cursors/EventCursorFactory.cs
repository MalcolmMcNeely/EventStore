using EventStore.EFCore.Postgres.Database;

namespace EventStore.EFCore.Postgres.Events.Cursors;

public class EventCursorFactory(EventStoreDbContext dbContext)
{
    internal async Task<EventCursorEntity> GetOrAddCursorAsync(string cursorName, CancellationToken token = default)
    {
        var cursor = await dbContext.EventCursorEntities.FindAsync([cursorName], token);

        if (cursor is not null)
        {
            return cursor;
        }

        cursor = new EventCursorEntity { CursorName = cursorName };

        await dbContext.EventCursorEntities.AddAsync(cursor, token);
        await dbContext.SaveChangesAsync(token);

        return cursor;
    }

    internal async Task SaveCursorAsync(EventCursorEntity eventCursorEntity, CancellationToken token = default)
    {
        dbContext.EventCursorEntities.Update(eventCursorEntity);
        await dbContext.SaveChangesAsync(token);
    }
}