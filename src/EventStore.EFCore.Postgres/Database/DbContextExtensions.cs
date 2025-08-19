using EventStore.Concurrency;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EventStore.EFCore.Postgres.Database;

public static class EventStoreDbContextExtensions
{
    public static async Task UpsertAsync<T>(this EventStoreDbContext dbContext, T entity, params object[] keyValues) where T : class, IConcurrencyCheck
    {
        try
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
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            var existing = await dbContext.FindAsync<T>(keyValues);
            if (existing != null)
            {
                entity.RowVersion = existing.RowVersion + 1;
                dbContext.Entry(existing).CurrentValues.SetValues(entity);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}