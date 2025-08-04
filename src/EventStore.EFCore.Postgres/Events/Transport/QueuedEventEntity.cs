using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventStore.EFCore.Postgres.Events.Transport;

public class QueuedEventEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public DateTime TimeStamp { get; set; }
    [MaxLength(128)]
    public required string EventType { get; set; }
    [Column(TypeName = "jsonb")]
    public required Envelope Envelope { get; set; }
    public int DequeueCount { get; set; }
}