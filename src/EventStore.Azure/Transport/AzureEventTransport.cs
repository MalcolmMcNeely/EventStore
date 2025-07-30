using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using EventStore.Azure.Transport.Events;
using EventStore.Azure.Transport.Events.Streams;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.Azure.Transport;

public class AzureEventTransport(AzureService azureService, AllStream allStream) : IEventTransport
{
    readonly QueueClient _queueClient = azureService.QueueServiceClient.GetQueueClient(Defaults.Transport.QueueName);

    public async Task PublishEventAsync<T>(T @event, CancellationToken token = default) where T : class, IEvent
    {
        await allStream.PublishAsync(Defaults.Streams.AllStreamPartition, @event, token);
    }

    public async Task<IEvent?> ReceiveEventAsync(CancellationToken token = default)
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
            throw new Exception($"Could not deserialize the message {message.MessageText}");
        }

        var type = Type.GetType(envelope.Type);
        var @event = JsonSerializer.Deserialize(envelope.Body, type);

        return @event as IEvent;
    }
}