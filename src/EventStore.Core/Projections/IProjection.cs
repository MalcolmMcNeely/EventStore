using System.ComponentModel.DataAnnotations;

namespace EventStore.Projections;

public interface IProjection
{
    [Key]
    public string Id { get; set; }
}