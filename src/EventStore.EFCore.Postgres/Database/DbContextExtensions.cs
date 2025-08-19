using EventStore.Concurrency;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EventStore.EFCore.Postgres.Database;

public static class EventStoreDbContextExtensions
{
    public static async Task UpsertAsync<T>(this EventStoreDbContext dbContext, T entity, object[] keyValues, CancellationToken token)
        where T : class, IConcurrencyCheck
    {
        try
        {
            var existing = await dbContext.FindAsync<T>(keyValues, token).ConfigureAwait(false);
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

            await dbContext.SaveChangesAsync(token).ConfigureAwait(false);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            dbContext.ChangeTracker.Clear();

            var existing = await dbContext.FindAsync<T>(keyValues, token).ConfigureAwait(false);

            entity.RowVersion = (existing?.RowVersion ?? 0 ) + 1;
            dbContext.Entry(existing!).CurrentValues.SetValues(entity);

            await dbContext.SaveChangesAsync(token).ConfigureAwait(false);
        }
    }
}