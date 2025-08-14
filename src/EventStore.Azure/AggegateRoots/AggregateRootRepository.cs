using System.Text.Json;
using Azure.Storage.Blobs;
using EventStore.AggregateRoots;
using EventStore.Azure.Extensions;
using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.Events.Transport;

namespace EventStore.Azure.AggegateRoots;

public class AggregateRootRepository<TAggregateRoot>(AzureService azureService, IEventTransport transport, IEventStreamFactory eventStreamFactory) : IAggregateRootRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot, new()
{
    readonly BlobContainerClient _blobContainerClient = azureService.BlobServiceClient.GetBlobContainerClient(Defaults.AggregateRoot.ContainerName);

    public async Task<TAggregateRoot> LoadAsync(string key, CancellationToken token = default)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(TAggregateRoot).FullName}/{key}");

        if (await blobClient.ExistsAsync(token))
        {
            return JsonSerializer.Deserialize<TAggregateRoot>(await blobClient.OpenReadAsync(cancellationToken: token))!;
        }

        return new TAggregateRoot { Id = key };
    }

    public async Task<bool> SaveAsync(TAggregateRoot aggregateRoot, string key, CancellationToken token = default)
    {
        var blobContent = JsonSerializer.Serialize(aggregateRoot);
        var binaryData = BinaryData.FromString(blobContent);
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(TAggregateRoot).FullName}/{key}");

        if (await blobClient.ExistsAsync(token) || !await blobClient.UploadOnlyIfNotCreated(binaryData, cancellationToken: token))
        {
            return await blobClient.UploadWithLeaseAsync(binaryData, token: token);
        }

        return true;
    }

    public async Task SendEventAsync<TEvent>(TEvent @event, string key, CancellationToken token = default) where TEvent : class, IEvent
    {
        // TODO: writing to aggregate root stream and all steam should be as one transaction
        var eventStream = eventStreamFactory.For($"{Defaults.AggregateRoot.AggregateRootPartitionPrefix}{key}");
        await eventStream.PublishAsync(@event, token);

        await transport.WriteEventAsync(@event, token);
    }
}