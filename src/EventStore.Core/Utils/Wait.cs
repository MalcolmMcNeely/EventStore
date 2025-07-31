namespace EventStore.Utils;

public static class Wait
{
    static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    public static async Task UntilAsync(Func<Task<bool>> condition, TimeSpan? timeout = null, CancellationToken token = default)
    {
        var start = DateTime.UtcNow;
        timeout ??= DefaultTimeout;

        while (!await condition())
        {
            if (DateTime.UtcNow - start > timeout.Value)
            {
                throw new TimeoutException("The condition was not met within the timeout period.");
            }

            token.ThrowIfCancellationRequested();

            await Task.Delay(50, token);
        }
    }
}