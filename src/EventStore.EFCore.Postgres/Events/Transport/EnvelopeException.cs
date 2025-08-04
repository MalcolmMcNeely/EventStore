namespace EventStore.EFCore.Postgres.Events.Transport;

public class EnvelopeException(string message) : Exception(message);