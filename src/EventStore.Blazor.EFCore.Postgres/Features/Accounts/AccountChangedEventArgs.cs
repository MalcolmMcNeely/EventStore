namespace EventStore.Blazor.EFCore.Postgres.Features.Accounts;

public record AccountChangedEventArgs(string AccountName, decimal Adjustment);