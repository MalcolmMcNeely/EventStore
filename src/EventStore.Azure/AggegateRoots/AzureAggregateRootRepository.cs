using System.Text.Json;
using Azure.Storage.Blobs;
using EventStore.Azure.Extensions;
using EventStore.Commands.AggregateRoots;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.Azure.AggegateRoots;

public class AzureAggregateRootRepository<TAggregateRoot>(AzureService azureService, IEventTransport transport) : IAggregateRootRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot, new()
{
    readonly BlobContainerClient _blobContainerClient = azureService.BlobServiceClient.GetBlobContainerClient(Defaults.AggregateRoot.ContainerName);

    public async Task<TAggregateRoot?> LoadAsync(string key, CancellationToken token = default)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(TAggregateRoot).FullName}/{key}");

        return !await blobClient.ExistsAsync(token) ? null : JsonSerializer.Deserialize<TAggregateRoot>(await blobClient.OpenReadAsync(cancellationToken: token));
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

    public async Task SendEventAsync<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : class, IEvent
    {
        await transport.PublishEventAsync(@event, token);
    }
}