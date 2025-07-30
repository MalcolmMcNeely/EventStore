using Azure.Data.Tables;

namespace EventStore.Azure.Transport.Cursors;

internal sealed class EventCursor(AzureService azureService)
{
    readonly TableClient _tableClient = azureService.TableServiceClient.GetTableClient(Defaults.Cursors.TableName);

    internal async Task CreateCursor(string cursorName, CancellationToken token = default)
    {
        var eventCursorEntity = new EventCursorEntity
        {
            PartitionKey = Defaults.Cursors.PartitionKey,
            RowKey = cursorName,
            LastSeenEvent = 0,
        };

        await _tableClient.UpsertEntityAsync(eventCursorEntity, TableUpdateMode.Replace, token);
    }

    internal async Task<EventCursorEntity?> GetCursor(string cursorName, CancellationToken token = default)
    {
        return (await _tableClient.GetEntityIfExistsAsync<EventCursorEntity?>(cursorName, cursorName, cancellationToken: token)).Value;
    }
}