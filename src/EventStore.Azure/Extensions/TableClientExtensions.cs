using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using EventStore.Azure.Azure;
using EventStore.Azure.Commands.TableEntities;
using EventStore.Azure.Events.TableEntities;

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

    public static async Task<MetadataEntity> GetMetadataEntityAsync(this TableClient tableClient, string streamName, CancellationToken token = default)
    {
        try
        {
            return await tableClient.GetEntityAsync<MetadataEntity>(streamName, RowKey.ForMetadata().ToString(), cancellationToken: token);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == TableErrorCode.ResourceNotFound)
        {
            return new MetadataEntity
            {
                PartitionKey = streamName,
                RowKey = RowKey.ForMetadata().ToString()
            };
        }
    }

    public static async Task UpdateCommandAuditAsync(this TableClient tableClient, CommandEntity commandEntity, MetadataEntity metadataEntity, CancellationToken token = default)
    {
        var transactions = new List<TableTransactionAction>
        {
            new(TableTransactionActionType.Add, commandEntity),
            new(TableTransactionActionType.UpsertReplace, metadataEntity)
        };

        await tableClient.SubmitTransactionAsync(transactions, token);
    }
}