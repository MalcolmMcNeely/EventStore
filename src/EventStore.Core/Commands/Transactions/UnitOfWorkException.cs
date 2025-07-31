namespace EventStore.Commands.Transactions;

public class UnitOfWorkException(string message) : Exception(message);