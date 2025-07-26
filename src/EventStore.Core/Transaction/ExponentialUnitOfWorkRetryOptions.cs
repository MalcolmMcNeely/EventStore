namespace EventStore.Transaction;

public record ExponentialUnitOfWorkRetryOptions(TimeSpan RetryInterval, int MaxRetries, double Exponential) : UnitOfWorkRetryOptions(RetryInterval, MaxRetries);