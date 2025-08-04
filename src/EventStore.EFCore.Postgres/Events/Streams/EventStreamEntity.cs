using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventStore.Events;
using Microsoft.EntityFrameworkCore;

namespace EventStore.EFCore.Postgres.Events.Streams;

[Index(nameof(Key), nameof(RowKey), IsUnique = true)]
public class EventStreamEntity
{
    [Key]
    public required string Key { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long RowKey { get; set; }
    public TimeSpan TimeStamp { get; set; }
    public required string EventType { get; set; }
    [Column(TypeName = "jsonb")]
    public required IEvent Content { get; set; }
}