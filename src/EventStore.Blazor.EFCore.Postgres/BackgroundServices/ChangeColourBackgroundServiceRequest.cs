namespace EventStore.Blazor.EFCore.Postgres.BackgroundServices;

public record ChangeColourBackgroundServiceRequest(bool? Running, bool? Toggle);