using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using EventStore.Azure.Events.TableEntities;
using EventStore.Azure.Extensions;
using EventStore.Events;

namespace EventStore.Azure.Events.Streams;

public class EventStream(AzureService azureService, string streamName, SemaphoreSlim semaphore)
{
    const int MaxRetries = 3;
    const int Exponential = 2;

    readonly TimeSpan _retryInterval = TimeSpan.FromMilliseconds(200);
    readonly TableClient _tableClient = azureService.TableServiceClient.GetTableClient(Defaults.Events.EventStoreTable);

    internal async Task PublishAsync(IEvent entity, CancellationToken token = default)
    {
        var eventType = entity.GetType();
        var content = JsonSerializer.Serialize((object)entity);
        var currentRetry = 0;

        await semaphore.WaitAsync(token);

        while (currentRetry < MaxRetries)
        {
            try
            {
                var metadataEntity = await _tableClient.GetMetadataEntityAsync(streamName, token: token);
                var eventEntity = new EventEntity
                {
                    PartitionKey = streamName,
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
