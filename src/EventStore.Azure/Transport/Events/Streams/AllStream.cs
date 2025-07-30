using EventStore.Events;

namespace EventStore.Azure.Transport.Events.Streams;

public class AllStream(AzureService azureService): EventStream(azureService)
{
    static readonly SemaphoreSlim Semaphore = new(1, 1);

    public async Task PublishAsync(string partitionKey, IEvent entity, CancellationToken token = default)
    {
        await PublishToStreamAsync(partitionKey, entity, Semaphore, token);
    }
}