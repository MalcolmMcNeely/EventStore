using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using EventStore.Commands.AggregateRoot;

namespace EventStore.Azure.AggegateRoot;

public class AzureAggregateRootRepository<T>(AzureService azureService) : IAggregateRootRepository<T> where T : AggregateRoot, new()
{
    readonly BlobContainerClient _blobContainerClient = azureService.BlobServiceClient.GetBlobContainerClient(BlobContainerConstants.AggregateRootContainerName);

    public async Task<T?> LoadAsync(string key)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).FullName}/{key}");

        return !await blobClient.ExistsAsync() ? null : JsonSerializer.Deserialize<T>(await blobClient.OpenReadAsync());
    }

    public async Task<bool> SaveAsync(T aggregateRoot, string key)
    {
        var blobContent = JsonSerializer.Serialize(aggregateRoot);
        var binaryData = BinaryData.FromString(blobContent);
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).FullName}/{key}");

        if (await blobClient.ExistsAsync() || !await blobClient.UploadOnlyIfNotCreated(binaryData))
        {
            return await blobClient.UploadWithLeaseAsync(binaryData);
        }

        return true;
    }
}