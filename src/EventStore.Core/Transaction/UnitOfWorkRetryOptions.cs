namespace EventStore.Transaction;

public record UnitOfWorkRetryOptions(TimeSpan RetryInterval, int MaxRetries);