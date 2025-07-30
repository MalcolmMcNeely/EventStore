using System.Text.Json;
using Azure.Storage.Blobs;
using EventStore.Azure.Extensions;
using EventStore.Projections;

namespace EventStore.Azure.Projections;

public class AzureProjectionRepository<T>(AzureService azureService) : IProjectionRepository<T>  where T : IProjection
{
    readonly BlobContainerClient _blobContainerClient = azureService.BlobServiceClient.GetBlobContainerClient(Defaults.Projections.ContainerName);

    public async Task<T?> LoadAsync(string key, CancellationToken token = default)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).Name}/{key}");

        return !await blobClient.ExistsAsync(token) ? default : JsonSerializer.Deserialize<T>(await blobClient.OpenReadAsync(cancellationToken: token));
    }

    public async Task SaveAsync(T projection, CancellationToken token = default)
    {
        var blobContent = JsonSerializer.Serialize(projection);
        var binaryData = BinaryData.FromString(blobContent);
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).Name}/{projection.Id}");

        if (await blobClient.ExistsAsync(token) || !await blobClient.UploadOnlyIfNotCreated(binaryData, cancellationToken: token))
        {
            await blobClient.UploadWithLeaseAsync(binaryData, token: token);
        }

        // need more advanced versioning here
    }
}