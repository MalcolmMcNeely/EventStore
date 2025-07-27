namespace EventStore.Commands.Transactions;

public record UnitOfWorkRetryOptions(TimeSpan RetryInterval, int MaxRetries);