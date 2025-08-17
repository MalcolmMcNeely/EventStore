namespace EventStore.Commands.Transactions;

public record ExponentialUnitOfWorkRetryOptions(TimeSpan RetryInterval, int MaxRetries, int Exponential) : UnitOfWorkRetryOptions(RetryInterval, MaxRetries)
{
    public override TimeSpan GetDelay(int currentRetry) => RetryInterval * currentRetry * Exponential;
}