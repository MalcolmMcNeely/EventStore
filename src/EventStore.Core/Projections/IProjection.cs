using System.ComponentModel.DataAnnotations;

namespace EventStore.Projections;

public interface IProjection
{
    [Key]
    public string Id { get; set; }
    [ConcurrencyCheck]
    public byte[] RowVersion { get; set; }
}