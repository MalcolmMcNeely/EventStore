using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventStore.EFCore.Postgres.Events.Transport;
using EventStore.Events;
using Microsoft.EntityFrameworkCore;

namespace EventStore.EFCore.Postgres.Events.Streams;

[Index(nameof(Key), nameof(RowKey), IsUnique = true)]
public class EventStreamEntity
{
    [Key]
    public required string Key { get; set; }
    public int RowKey { get; set; }
    public DateTime TimeStamp { get; set; }
    public required string EventType { get; set; }
    [Column(TypeName = "jsonb")]
    public required Envelope Envelope { get; set; }
}