using EventStore.EFCore.Postgres.Database;
using EventStore.Projections;
using Microsoft.EntityFrameworkCore;

namespace EventStore.EFCore.Postgres.Projections;

public class ProjectionRepository<T>(EventStoreDbContext dbContext, ProjectionRebuilder projectionRebuilder) : IProjectionRepository<T> where T : class, IProjection, new()
{
    public async Task<T> LoadAsync(string key, CancellationToken token = default)
    {
        var entity = await dbContext.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == key, token)
            .ConfigureAwait(false);

        return entity ?? new T { Id = key };
    }

    public async Task SaveAsync(T projection, CancellationToken token = default)
    {
        await dbContext.UpsertAsync(projection, projection.Id).ConfigureAwait(false);
    }
}