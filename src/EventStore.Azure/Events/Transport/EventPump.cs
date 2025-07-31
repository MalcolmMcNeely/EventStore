using System.Runtime.CompilerServices;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using EventStore.Azure.Events.Cursors;
using EventStore.Azure.Events.TableEntities;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.Azure.Events.Transport;

public class EventPump(AzureService azureService, EventCursorFactory eventCursorFactory, EventTypeRegistration eventTypeRegistration) : IEventPump
{
    readonly TableClient _eventsTable = azureService.TableServiceClient.GetTableClient(Defaults.Events.EventStoreTable);
    readonly QueueClient _transportQueue = azureService.QueueServiceClient.GetQueueClient(Defaults.Transport.QueueName);

    public async Task PublishEventsAsync(CancellationToken token = default)
    {
        var cursor = await eventCursorFactory.GetOrAddCursorAsync(Defaults.Cursors.AllStreamCursor, token);
        string? continuationToken = null;
        var eventCount = 0;

        do
        {
            var pages = ReceiveEventsAsync(cursor, continuationToken, token);

            await foreach (var page in pages)
            {
                foreach (var eventEntity in page.Values)
                {
                    var eventType = eventTypeRegistration.EventNameToTypeMap[eventEntity.EventType];
                    var @event = JsonSerializer.Deserialize(eventEntity.Content, eventType);
                    var transportEnvelope = TransportEnvelope.Create(@event!);
                    await _transportQueue.SendMessageAsync(JsonSerializer.Serialize(transportEnvelope), token);
                    eventCount++;
                }

                continuationToken = page.ContinuationToken;
            }
        } while (continuationToken is not null);

        cursor.LastSeenEvent += eventCount;
        await eventCursorFactory.SaveCursorAsync(cursor, token);
    }

    async IAsyncEnumerable<Page<EventEntity>> ReceiveEventsAsync(EventCursorEntity eventCursor, string? continuationToken = null, [EnumeratorCancellation] CancellationToken token = default)
    {
        var pages = _eventsTable.QueryAsync<EventEntity>(
                $"PartitionKey eq '{Defaults.Streams.AllStreamPartition}' and RowKey gt '{RowKey.ForEventStream(eventCursor.LastSeenEvent)}'",
                cancellationToken: token)
            .AsPages(continuationToken, 5);

        await foreach (var page in pages)
        {
            yield return page;
        }
    }
}