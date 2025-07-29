using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.Azure.Transport;

public class AzureEventTransport(AzureService azureService) : IEventTransport
{
    readonly QueueClient _queueClient = azureService.QueueServiceClient.GetQueueClient(QueueConstants.TransportQueueName);

    public async Task SendEventAsync<T>(T @event, CancellationToken token = default) where T : class, IEvent
    {
        var envelope = TransportEnvelope.Create(@event);

        await _queueClient.SendMessageAsync(JsonSerializer.Serialize(envelope), token);
    }

    public async Task<IEvent?> GetEventAsync(CancellationToken token = default)
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