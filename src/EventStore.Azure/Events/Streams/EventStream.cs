﻿using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using EventStore.Azure.Events.TableEntities;
using EventStore.Azure.Extensions;
using EventStore.Events;
using EventStore.Events.Streams;

namespace EventStore.Azure.Events.Streams;

public class EventStream(AzureService azureService, string streamName, SemaphoreSlim semaphore, Lazy<IEventTypeRegistration> eventTypeRegistration) : IEventStream
{
    const int MaxRetries = 3;
    const int Exponential = 2;

    readonly TimeSpan _retryInterval = TimeSpan.FromMilliseconds(200);
    readonly TableClient _tableClient = azureService.TableServiceClient.GetTableClient(Defaults.Events.EventStoreTable);

    public async Task PublishAsync(IEvent entity, CancellationToken token = default)
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

    public async Task<bool> ExistsAsync(CancellationToken token = default)
    {
        var response = await _tableClient.GetEntityIfExistsAsync<MetadataEntity>(streamName, RowKey.ForMetadata().ToString(), cancellationToken: token);
        return response.HasValue;
    }

    public async IAsyncEnumerable<IEvent> GetAllEventsAsync(CancellationToken token = default)
    {
        var events = _tableClient.QueryAsync<EventEntity>(x => x.PartitionKey == streamName && x.RowKey != RowKey.ForMetadata().ToString());

        await foreach (var entity in events)
        {
            var eventType = eventTypeRegistration.Value.EventNameToTypeMap[entity.EventType];
            var @event = JsonSerializer.Deserialize(entity.Content, eventType);

            yield return (IEvent)@event!;
        }
    }
}
