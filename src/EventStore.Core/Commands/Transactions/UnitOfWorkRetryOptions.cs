namespace EventStore.Commands.Transactions;

public record UnitOfWorkRetryOptions(TimeSpan RetryInterval, int MaxRetries)
{
    public virtual TimeSpan GetDelay(int currentRetry) => RetryInterval;
}