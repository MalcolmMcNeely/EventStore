using System.ComponentModel.DataAnnotations;

namespace EventStore.Concurrency;

public interface IConcurrencyCheck
{
    [ConcurrencyCheck]
    public int RowVersion { get; set; }
}