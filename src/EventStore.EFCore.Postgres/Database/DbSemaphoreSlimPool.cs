namespace EventStore.EFCore.Postgres.Database;

public sealed class DbSemaphoreSlimPool : IDisposable
{
    const int MaxConcurrency = 97;
    static SemaphoreSlim SlimPool { get; } = new(MaxConcurrency);
    bool _disposed;

    public static async Task<DbSemaphoreSlimPool> AcquireAsync(CancellationToken token)
    {
        await SlimPool.WaitAsync(token).ConfigureAwait(false);
        return new DbSemaphoreSlimPool();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            SlimPool.Release();
            _disposed = true;
        }
    }
}