namespace EventStore.Blazor.EFCore.Postgres.Models;

public record EventModel(int Id, string Stream, DateTime Time, string EventType, string Content);