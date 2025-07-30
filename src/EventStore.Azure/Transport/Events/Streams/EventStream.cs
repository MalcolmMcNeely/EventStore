using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using EventStore.Azure.Extensions;
using EventStore.Azure.Transport.Events.TableEntities;
using EventStore.Events;

namespace EventStore.Azure.Transport.Events.Streams;

public abstract class EventStream(AzureService azureService)
{
    const int MaxRetries = 3;
    const int Exponential = 2;

    readonly TimeSpan _retryInterval = TimeSpan.FromMilliseconds(200);
    readonly TableClient _tableClient = azureService.TableServiceClient.GetTableClient(Defaults.Events.EventStoreTable);

    protected async Task PublishToStreamAsync(string partitionKey, IEvent entity, SemaphoreSlim semaphore, CancellationToken token = default)
    {
        var eventType = entity.GetType();
        var content = JsonSerializer.Serialize((object)entity);
        var currentRetry = 0;

        await semaphore.WaitAsync(token);

        while (currentRetry < MaxRetries)
        {
            try
            {
                var metadataEntity = await _tableClient.GetMetadataEntityAsync(Defaults.Streams.AllStreamPartition, token: token);
                var eventEntity = new EventEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = RowKey.ForEventStream(metadataEntity.LastEvent + 1).ToString(),
                    EventType = eventType.Name,
                    IsLarge = false,
                    Content = content
                };
                metadataEntity.LastEvent++;

                await _tableClient.UpdateAllStreamAsync(eventEntity, metadataEntity, token);

                break;
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == TableErrorCode.EntityAlreadyExists)
            {
                currentRetry++;

                await Task.Delay(_retryInterval * currentRetry * Exponential, token);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
