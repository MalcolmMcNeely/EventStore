using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using EventStore.Azure.Azure;
using EventStore.Azure.Commands.TableEntities;
using EventStore.Azure.Extensions;
using EventStore.Commands;
using EventStore.Commands.Dispatching;

namespace EventStore.Azure.Commands;

public class CommandAudit(AzureService azureService) : ICommandAudit
{
    const int MaxRetries = 3;
    const int Exponential = 2;

    readonly TimeSpan _retryInterval = TimeSpan.FromMilliseconds(200);
    readonly TableClient _tableClient = azureService.TableServiceClient.GetTableClient(Defaults.Commands.CommandsTable);
    readonly SemaphoreSlim semaphore = new(1, 1);

    public async Task PublishAsync<T>(T command, CancellationToken token) where T : ICommand
    {
        var commandType = command.GetType();
        var content = JsonSerializer.Serialize((object)command);
        var currentRetry = 0;

        await semaphore.WaitAsync(token);

        while (currentRetry < MaxRetries)
        {
            try
            {
                var metadataEntity = await _tableClient.GetMetadataEntityAsync(Defaults.Commands.CommandPartitionKey, token);
                var commandEntity = new CommandEntity
                {
                    PartitionKey = Defaults.Commands.CommandPartitionKey,
                    RowKey = RowKey.ForEventStream(metadataEntity.LastEvent + 1).ToString(),
                    CommandType = commandType.Name,
                    CausationId = command.CausationId,
                    Content = content
                };
                metadataEntity.LastEvent++;

                await _tableClient.UpdateCommandAuditAsync(commandEntity, metadataEntity, token);

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