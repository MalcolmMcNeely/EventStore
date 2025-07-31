using Azure;
using Azure.Data.Tables;

namespace EventStore.Azure.Events.Cursors;

public sealed class EventCursorEntity : ITableEntity
{
    public int LastSeenEvent { get; set; }

    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}