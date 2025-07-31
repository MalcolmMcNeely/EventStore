namespace EventStore.Commands.Dispatching;

public class CommandDispatcherException(string message) : Exception(message);