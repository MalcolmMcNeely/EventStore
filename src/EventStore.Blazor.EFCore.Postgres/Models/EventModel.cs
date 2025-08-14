namespace EventStore.Blazor.EFCore.Postgres.Components.Models;

public record EventModel(int Id, string Stream, DateTime Time, string EventType, string Content);