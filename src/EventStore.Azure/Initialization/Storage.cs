using EventStore.Utils;
using Microsoft.Extensions.Logging;

namespace EventStore.Azure.Initialization;

public class Storage(
    AzureService azureService,
    IEnumerable<IStorageInitializer> storageInitializers,
    ILogger<Storage> logger)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var blobServiceClient = azureService.BlobServiceClient;
        var tableServiceClient = azureService.TableServiceClient;
        var queueServiceClient = azureService.QueueServiceClient;

        foreach (var initializer in storageInitializers)
        {
            foreach (var containerInitializer in initializer.ContainerInitializers)
            {
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerInitializer.Name);

                if (await blobContainerClient.ExistsAsync(cancellationToken))
                {
                    continue;
                }

                logger.LogInformation($"Creating blob container {containerInitializer.Name}");

                await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            }

            foreach (var tableInitializer in initializer.TableInitializers)
            {
                var tableClient = tableServiceClient.GetTableClient(tableInitializer.Name);
                var response = await tableClient.CreateIfNotExistsAsync(cancellationToken);

                if (response.GetRawResponse().Status == Defaults.Tables.TableCreatedResponseCode)
                {
                    logger.LogInformation($"Creating table {tableInitializer.Name}");
                }
            }

            foreach (var queueInitializer in initializer.QueueInitializers)
            {
                var queueClient = queueServiceClient.GetQueueClient(queueInitializer.Name);

                if (await queueClient.ExistsAsync(cancellationToken))
                {
                    continue;
                }

                logger.LogInformation($"Creating queue {queueInitializer.Name}");

                await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            }
        }
    }

    public async Task DestroyAsync(CancellationToken cancellationToken = default)
    {
        var blobServiceClient = azureService.BlobServiceClient;
        var tableServiceClient = azureService.TableServiceClient;
        var queueServiceClient = azureService.QueueServiceClient;

        foreach (var initializer in storageInitializers)
        {
            foreach (var containerInitializer in initializer.ContainerInitializers)
            {
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerInitializer.Name);

                if (!await blobContainerClient.ExistsAsync(cancellationToken))
                {
                    continue;
                }

                logger.LogInformation($"Deleting blob container {containerInitializer.Name}");

                await blobContainerClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

                await Wait.UntilAsync(async () => !await blobContainerClient.ExistsAsync(cancellationToken), token: cancellationToken);
            }

            foreach (var tableInitializer in initializer.TableInitializers)
            {
                var tableClient = tableServiceClient.GetTableClient(tableInitializer.Name);
                await tableClient.DeleteAsync(cancellationToken);
            }

            await Wait.UntilAsync(async () =>
            {
                var tables = await tableServiceClient
                    .QueryAsync(cancellationToken: cancellationToken)
                    .ToListAsync(cancellationToken: cancellationToken);

                return tables.Count == 0;
            }, token: cancellationToken);

            foreach (var queueInitializer in initializer.QueueInitializers)
            {
                var queueClient = queueServiceClient.GetQueueClient(queueInitializer.Name);

                if (!await queueClient.ExistsAsync(cancellationToken))
                {
                    continue;
                }

                logger.LogInformation($"Deleting queue {queueInitializer.Name}");

                await queueClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
                await Wait.UntilAsync(async () => !await queueClient.ExistsAsync(cancellationToken), token: cancellationToken);
            }
        }
    }
}