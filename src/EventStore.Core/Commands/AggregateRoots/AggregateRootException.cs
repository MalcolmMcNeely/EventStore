namespace EventStore.Commands.AggregateRoots;

public class AggregateRootException(string message) : Exception(message);