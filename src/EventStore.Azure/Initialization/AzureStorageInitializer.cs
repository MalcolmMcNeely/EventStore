namespace EventStore.Azure.Initialization;

public class AzureStorageInitializer : IStorageInitializer
{
    public IEnumerable<ContainerInitializer> ContainerInitializers { get; } =
    [
        new(Defaults.Events.LargeEventContainerName),
        new(Defaults.AggregateRoot.ContainerName),
        new(Defaults.Projections.ContainerName)
    ];

    public IEnumerable<TableInitializer> TableInitializers { get; } =
    [
        new(Defaults.Cursors.TableName),
        new(Defaults.Events.EventStoreTable)
    ];

    public IEnumerable<QueueInitializer> QueueInitializers { get; } =
    [
        new(Defaults.Transport.QueueName)
    ];
}