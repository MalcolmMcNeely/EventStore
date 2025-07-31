namespace EventStore.Azure.Events.Transport;

public class TransportEnvelopeException(string message) : Exception(message);