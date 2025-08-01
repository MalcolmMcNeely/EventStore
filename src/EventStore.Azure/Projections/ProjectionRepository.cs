using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EventStore.Azure.Extensions;
using EventStore.Projections;

namespace EventStore.Azure.Projections;

public class ProjectionRepository<T>(AzureService azureService, ProjectionRebuilder projectionRebuilder) : IProjectionRepository<T>  where T : IProjection, new()
{
    readonly BlobContainerClient _blobContainerClient = azureService.BlobServiceClient.GetBlobContainerClient(Defaults.Projections.ContainerName);

    public async Task<T?> LoadAsync(string key, CancellationToken token = default)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).Name}/{key}");

        if (!await blobClient.ExistsAsync(token))
        {
            if (await projectionRebuilder.CanRebuildAsync(key, token))
            {
                return await projectionRebuilder.RebuildAsync<T>(key, token);
            }

            return default;
        }

        try
        {
            var blobStream = await blobClient.OpenReadAsync(cancellationToken: token);
            return JsonSerializer.Deserialize<T>(blobStream);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.ConditionNotMet)
        {
            await Task.Delay(50, token); // TODO: backoff, revisit Retry logic
        }

        return default;
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
    }
}