using EventStore.EFCore.Postgres.Database;
using EventStore.Projections;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Registry;

namespace EventStore.EFCore.Postgres.Projections;

public sealed class ProjectionRepository<T>(EventStoreDbContext dbContext, ProjectionRebuilder projectionRebuilder) : IProjectionRepository<T> where T : class, IProjection, new()
{
    public async Task<T> LoadAsync(string key, CancellationToken token = default)
    {
        using var semaphorePool = await DbSemaphoreSlimPool.AcquireAsync(token);

        var entity = await dbContext.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == key, token)
            .ConfigureAwait(false);

        return entity ?? new T { Id = key };
    }

    public async Task SaveAsync(T projection, CancellationToken token = default)
    {
        using var semaphorePool = await DbSemaphoreSlimPool.AcquireAsync(token);

        await dbContext.UpsertAsync(projection, [projection.Id], token).ConfigureAwait(false);
    }
}