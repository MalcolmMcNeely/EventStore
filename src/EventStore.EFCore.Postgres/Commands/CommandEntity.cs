using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EventStore.EFCore.Postgres.Commands;

[Index(nameof(Key), nameof(RowKey), IsUnique = true)]
public class CommandEntity
{
    [Key]
    [MaxLength(128)]
    public required string Key { get; set; }
    public int RowKey { get; set; }
    public DateTime TimeStamp { get; set; }
    //[MaxLength(128)]
    public required string CommandType { get; set; }
    public required string CausationId { get; set; }
    public required string Content { get; set; }
}