using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventStore.Events;

namespace EventStore.EFCore.Postgres.Events.Transport;

public class QueuedEventEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public TimeSpan TimeStamp { get; set; }
    public required string EventType { get; set; }
    [Column(TypeName = "jsonb")]
    public required IEvent Content { get; set; }
    public int DequeueCount { get; set; }
}