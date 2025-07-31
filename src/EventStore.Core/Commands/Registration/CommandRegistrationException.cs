namespace EventStore.Commands.Registration;

public class CommandRegistrationException(string message) : Exception(message);