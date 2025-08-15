namespace EventStore.Utility;

public static class Retry
{
    static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    public static async Task<T> ExecuteAsync<T>(Func<Task<T>> asyncAction, TimeSpan? timeout = null, CancellationToken token = default)
    {
        var start = DateTime.UtcNow;
        timeout ??= DefaultTimeout;

        while (true)
        {
            try
            {
                return await asyncAction().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // ignored
            }

            if (DateTime.UtcNow - start > timeout.Value)
            {
                break;
            }

            token.ThrowIfCancellationRequested();

            await Task.Delay(50, token);
        }

        throw new TimeoutException();
    }
}