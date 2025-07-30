using Azure;
using Azure.Data.Tables;

namespace EventStore.Azure.Transport.Events.TableEntities;

public class TableEntity : ITableEntity
{
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}