namespace EventStore.EFCore.Postgres;

public class StartupException(string message) : Exception(message);