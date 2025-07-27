using System.Text.Json;
using Azure.Storage.Blobs;
using EventStore.Commands.AggregateRoot;
using EventStore.Events;
using EventStore.Transport;

namespace EventStore.Azure.AggegateRoot;

public class AzureAggregateRootRepository<T>(AzureService azureService, IEventTransport transport) : IAggregateRootRepository<T> where T : AggregateRoot, new()
{
    readonly BlobContainerClient _blobContainerClient = azureService.BlobServiceClient.GetBlobContainerClient(BlobContainerConstants.AggregateRootContainerName);

    public async Task<T?> LoadAsync(string key, CancellationToken token = default)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).FullName}/{key}");

        return !await blobClient.ExistsAsync(token) ? null : JsonSerializer.Deserialize<T>(await blobClient.OpenReadAsync(cancellationToken: token));
    }

    public async Task<bool> SaveAsync(T aggregateRoot, string key, CancellationToken token = default)
    {
        var blobContent = JsonSerializer.Serialize(aggregateRoot);
        var binaryData = BinaryData.FromString(blobContent);
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).FullName}/{key}");

        if (await blobClient.ExistsAsync(token) || !await blobClient.UploadOnlyIfNotCreated(binaryData, cancellationToken: token))
        {
            return await blobClient.UploadWithLeaseAsync(binaryData, token: token);
        }

        return true;
    }

    public async Task SendEventsAsync(IEnumerable<IEvent> events, CancellationToken token = default)
    {
        foreach (var @event in events)
        {
            await transport.SendEventAsync(@event, token);
        }
    }
}