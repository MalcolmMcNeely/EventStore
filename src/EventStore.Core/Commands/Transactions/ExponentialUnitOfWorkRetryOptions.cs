namespace EventStore.Commands.Transactions;

public record ExponentialUnitOfWorkRetryOptions(TimeSpan RetryInterval, int MaxRetries, int Exponential) : UnitOfWorkRetryOptions(RetryInterval, MaxRetries);