using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventStore.EFCore.Postgres.Events.Transport;
using Microsoft.EntityFrameworkCore;

namespace EventStore.EFCore.Postgres.Events.Streams;

[Index(nameof(Key), nameof(RowKey), IsUnique = true)]
public class EventStreamEntity
{
    [Key]
    [MaxLength(128)]
    public required string Key { get; set; }
    public int RowKey { get; set; }
    public DateTime TimeStamp { get; set; }
    [MaxLength(128)]
    public required string EventType { get; set; }
    public required string CausationId { get; set; }
    [Column(TypeName = "jsonb")]
    public required Envelope Envelope { get; set; }
}