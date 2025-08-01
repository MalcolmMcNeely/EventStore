namespace EventStore.EFCore.Postgres.Events.Cursors;

public sealed class EventCursor
{
    public int LastSeenEvent { get; set; }

    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}