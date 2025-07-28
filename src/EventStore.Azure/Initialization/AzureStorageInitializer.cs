namespace EventStore.Azure.Initialization;

public class AzureStorageInitializer : IStorageInitializer
{
    public IEnumerable<ContainerInitializer> ContainerInitializers { get; } =
    [
        new(BlobContainerConstants.LargeEventContainerName),
        new(BlobContainerConstants.AggregateRootContainerName),
        new(BlobContainerConstants.ProjectionContainerName)
    ];

    public IEnumerable<TableInitializer> TableInitializers { get; } =
    [
        new(TableConstants.EventCursorsTableName),
        new(TableConstants.EventTableName)
    ];

    public IEnumerable<QueueInitializer> QueueInitializers { get; } =
    [
        new(QueueConstants.TransportQueueName)
    ];
}