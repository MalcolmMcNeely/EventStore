using Azure.Data.Tables;
using EventStore.Azure.Transport.Events.TableEntities;

namespace EventStore.Azure.Extensions;

public static class TableClientExtensions
{
    public static async Task UpdateAllStreamAsync(this TableClient tableClient, EventEntity eventEntity, MetadataEntity metadataEntity, CancellationToken token = default)
    {
        var transactions = new List<TableTransactionAction>
        {
            new(TableTransactionActionType.Add, eventEntity),
            new(TableTransactionActionType.UpsertReplace, metadataEntity)
        };

        await tableClient.SubmitTransactionAsync(transactions, token);
    }
}