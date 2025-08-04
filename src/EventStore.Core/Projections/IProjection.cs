using System.ComponentModel.DataAnnotations;
using EventStore.Concurrency;

namespace EventStore.Projections;

public interface IProjection : IConcurrencyCheck
{
    [Key]
    public string Id { get; set; }
}