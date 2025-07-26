using System.Text.Json;
using Azure.Storage.Blobs;
using EventStore.Commands.AggregateRoot;

namespace EventStore.Azure.AggegateRoot;

public class AzureEventSourcedEntityRepository<T>(AzureService azureService) : IEventSourcedEntityRepository<T> where T : AggregateRoot, new()
{
    readonly BlobContainerClient _blobContainerClient = azureService.BlobServiceClient.GetBlobContainerClient(BlobContainerConstants.AggregateRootContainerName);

    public T? Load(string key)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).FullName}/{key}");

        return !blobClient.Exists() ? null : JsonSerializer.Deserialize<T>(blobClient.OpenRead());
    }

    public void Save(T aggregateRoot, string key)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).FullName}/{key}");
        var blobContent = JsonSerializer.Serialize(aggregateRoot);

        blobClient.Upload(BinaryData.FromString(blobContent));
    }
}