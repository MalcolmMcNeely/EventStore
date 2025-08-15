using Azure.Data.Tables;
using EventStore.Azure.Azure;

namespace EventStore.Azure.Events.Cursors;

public sealed class EventCursorFactory(AzureService azureService)
{
    readonly TableClient _tableClient = azureService.TableServiceClient.GetTableClient(Defaults.Cursors.TableName);

    internal async Task<EventCursorEntity> GetOrAddCursorAsync(string cursorName, CancellationToken token = default)
    {
        var existingCursorEntity = await _tableClient.GetEntityIfExistsAsync<EventCursorEntity?>(Defaults.Cursors.PartitionKey, cursorName, cancellationToken: token).ConfigureAwait(false);
        if (existingCursorEntity.HasValue)
        {
            return existingCursorEntity.Value!;
        }

        var eventCursorEntity = new EventCursorEntity
        {
            PartitionKey = Defaults.Cursors.PartitionKey,
            RowKey = cursorName,
            LastSeenEvent = 0,
        };

        await _tableClient.UpsertEntityAsync(eventCursorEntity, TableUpdateMode.Replace, token).ConfigureAwait(false);

        return eventCursorEntity;
    }

    internal async Task SaveCursorAsync(EventCursorEntity eventCursorEntity, CancellationToken token = default)
    {
        await _tableClient.UpsertEntityAsync(eventCursorEntity, TableUpdateMode.Replace, token).ConfigureAwait(false);
    }
}