using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EventStore.Azure.Azure;
using EventStore.Azure.Extensions;
using EventStore.Projections;
using Polly;
using Polly.Registry;

namespace EventStore.Azure.Projections;

public sealed class ProjectionRepository<T>(AzureService azureService, ProjectionRebuilder projectionRebuilder, ResiliencePipelineProvider<string> resiliencePipelineProvider) : IProjectionRepository<T>  where T : IProjection, new()
{
    readonly BlobContainerClient _blobContainerClient = azureService.BlobServiceClient.GetBlobContainerClient(Defaults.Projections.ContainerName);
    readonly ResiliencePipeline _pipeline = resiliencePipelineProvider.GetPipeline(Defaults.Resilience.DefaultAzurePipeline);

    public async Task<T> LoadAsync(string key, CancellationToken token = default)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).Name}/{key}");

        if (!await blobClient.ExistsAsync(token).ConfigureAwait(false))
        {
            if (await projectionRebuilder.CanRebuildAsync(key, token).ConfigureAwait(false))
            {
                return await projectionRebuilder.RebuildAsync<T>(key, token).ConfigureAwait(false);
            }

            return new T { Id = key };
        }

        try
        {
            var blobStream = await _pipeline.ExecuteAsync(
                static async (bc, ct) => await bc.OpenReadAsync(cancellationToken: ct).ConfigureAwait(false), blobClient, token)
                .ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(blobStream)!;
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.ConditionNotMet)
        {
            return new T { Id = key };
        }
    }

    public async Task SaveAsync(T projection, CancellationToken token = default)
    {
        var blobContent = JsonSerializer.Serialize(projection);
        var binaryData = BinaryData.FromString(blobContent);
        var blobClient = _blobContainerClient.GetBlobClient($"{typeof(T).Name}/{projection.Id}");

        if (await blobClient.ExistsAsync(token).ConfigureAwait(false) || !await blobClient.UploadOnlyIfNotCreated(binaryData, cancellationToken: token).ConfigureAwait(false))
        {
            await blobClient.UploadWithLeaseAsync(binaryData, token: token).ConfigureAwait(false);
        }
    }
}