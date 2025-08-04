using System.ComponentModel.DataAnnotations;

namespace EventStore.EFCore.Postgres.Events.Cursors;

public sealed class EventCursorEntity
{
    [Key]
    public required string CursorName { get; set; }
    public DateTime Timestamp { get; set; }
    public long LastSeenEvent { get; set; }
}