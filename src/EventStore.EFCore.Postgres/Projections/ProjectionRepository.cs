using EventStore.Projections;

namespace EventStore.EFCore.Postgres.Projections;

public class ProjectionRepository<T>(EventStoreDbContext dbContext, ProjectionRebuilder projectionRebuilder) : IProjectionRepository<T>  where T : class, IProjection, new()
{
    public async Task<T> LoadAsync(string key, CancellationToken token = default)
    {
        return await dbContext.FindAsync<T>([key], token) ?? new T();
    }

    public async Task SaveAsync(T projection, CancellationToken token = default)
    {
        dbContext.Update(projection);
        await dbContext.SaveChangesAsync(token);
    }
}