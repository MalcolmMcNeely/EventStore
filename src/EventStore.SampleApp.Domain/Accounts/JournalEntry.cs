namespace EventStore.SampleApp.Domain.Accounts;

public record JournalEntry(DateTime description, List<JournalLine> Lines);