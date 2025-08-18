namespace EventStore.SampleApp.Domain.Accounts;

public record AccountModel(string Name, AccountType Type, decimal Balance, string CreatedBy);