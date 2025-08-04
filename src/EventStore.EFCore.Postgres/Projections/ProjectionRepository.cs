using EventStore.Projections;
using Microsoft.EntityFrameworkCore;

namespace EventStore.EFCore.Postgres.Projections;

public class ProjectionRepository<T>(EventStoreDbContext dbContext, ProjectionRebuilder projectionRebuilder) : IProjectionRepository<T> where T : class, IProjection, new()
{
    public async Task<T> LoadAsync(string key, CancellationToken token = default)
    {
        return await dbContext.FindAsync<T>([key], token) ?? new T { Id = key };
    }

    public async Task SaveAsync(T projection, CancellationToken token = default)
    {
        try
        {
            dbContext.Update(projection);
            await dbContext.SaveChangesAsync(token);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            // TODO: Need to think about this more
            foreach (var entry in exception.Entries)
            {
                var proposedValues = entry.CurrentValues;
                entry.OriginalValues.SetValues(proposedValues);
            }
        }
    }
}