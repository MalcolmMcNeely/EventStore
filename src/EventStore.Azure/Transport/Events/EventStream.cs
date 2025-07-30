using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using EventStore.Azure.Extensions;
using EventStore.Azure.Transport.Events.TableEntities;
using EventStore.Events;

namespace EventStore.Azure.Transport.Events;

public class EventStream(AzureService azureService)
{
    static SemaphoreSlim _semaphore = new(1, 1);

    const int MaxRetries = 3;
    const int Exponential = 2;

    readonly TimeSpan _retryInterval = TimeSpan.FromMilliseconds(200);
    readonly TableClient _tableClient = azureService.TableServiceClient.GetTableClient(Defaults.Events.EventStoreTable);

    public async Task PublishAsync(IEvent entity, CancellationToken token = default)
    {
        var eventType = entity.GetType();
        var content = JsonSerializer.Serialize((object)entity);
        var currentRetry = 0;

        await _semaphore.WaitAsync(token);

        while (currentRetry < MaxRetries)
        {

            try
            {
                var metadataEntity = await GetMetadataEntityAsync(token);
                var eventEntity = new EventEntity
                {
                    PartitionKey = Defaults.Streams.AllStreamPartition,
                    RowKey = RowKey.ForAllStream(metadataEntity.LastEvent + 1).ToString(),
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
                _semaphore.Release();
            }
        }
    }

    async Task<MetadataEntity> GetMetadataEntityAsync(CancellationToken token = default)
    {
        try
        {
            return await _tableClient.GetEntityAsync<MetadataEntity>(
                Defaults.Streams.AllStreamPartition, RowKey.ForMetadata().ToString(), cancellationToken: token);

        }
        catch (RequestFailedException ex) when (ex.ErrorCode == TableErrorCode.ResourceNotFound)
        {
            return new MetadataEntity
            {
                PartitionKey = Defaults.Streams.AllStreamPartition,
                RowKey = RowKey.ForMetadata().ToString()
            };
        }
    }
}
