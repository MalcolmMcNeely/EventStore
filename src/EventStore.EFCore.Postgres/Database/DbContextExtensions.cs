using EventStore.Concurrency;

namespace EventStore.EFCore.Postgres.Database;

public static class EventStoreDbContextExtensions
{
    public static async Task UpsertAsync<T>(this EventStoreDbContext dbContext, T entity, params object[] keyValues) where T : class, IConcurrencyCheck
    {
        var existing = await dbContext.FindAsync<T>(keyValues).ConfigureAwait(false);
        if (existing is null)
        {
            entity.RowVersion = 0;
            dbContext.Add(entity);
        }
        else
        {
            entity.RowVersion = existing.RowVersion + 1;
            dbContext.Entry(existing).CurrentValues.SetValues(entity);
        }

        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}