using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.Azure.Events.Transport;

public class AzureEventBroadcaster(AzureService azureService, EventDispatcher eventDispatcher) : IEventBroadcaster
{
    readonly QueueClient _queueClient = azureService.QueueServiceClient.GetQueueClient(Defaults.Transport.QueueName);

    public async Task BroadcastEventAsync(CancellationToken token = default)
    {
        var @event = await ReceiveEventAsync(token);
        
        if (@event is not null)
        {
            await eventDispatcher.SendEventAsync(@event, token);
        }
    }

    async Task<IEvent?> ReceiveEventAsync(CancellationToken token = default)
    {
        QueueMessage message = await _queueClient.ReceiveMessageAsync(cancellationToken: token);

        if (message is null)
        {
            return null;
        }

        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, token);

        if (message.MessageText is null)
        {
            return null;
        }

        var envelope = JsonSerializer.Deserialize<TransportEnvelope>(message.MessageText);

        if (envelope is null)
        {
            throw new AzureEventBroadcasterException($"Could not deserialize the message {message.MessageText}");
        }

        var type = Type.GetType(envelope.Type);
        var @event = JsonSerializer.Deserialize(envelope.Body, type!);

        return @event as IEvent;
    }
}