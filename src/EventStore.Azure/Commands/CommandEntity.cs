using EventStore.Azure.Azure;

namespace EventStore.Azure.Commands;

public class CommandEntity : TableEntity
{
    public required string CommandType { get; set; }
    public required string CausationId { get; set; }
    public required string Content { get; set; }
}