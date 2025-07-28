namespace EventStore.Azure.Initialization;

public record ContainerInitializer(string Name);
public record TableInitializer(string Name);
public record QueueInitializer(string Name);

public interface IStorageInitializer
{
    public IEnumerable<ContainerInitializer> ContainerInitializers { get; }
    public IEnumerable<TableInitializer> TableInitializers { get; }
    public IEnumerable<QueueInitializer> QueueInitializers { get; }
}