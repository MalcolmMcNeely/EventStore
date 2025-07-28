using System.Text.Json;
using Azure.Storage.Queues;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.Azure;

public class AzureEventTransport(AzureService azureService) : IEventTransport
{
    QueueClient queueClient = azureService.QueueServiceClient.GetQueueClient(QueueConstants.TransportQueueName);

    public async Task SendEventAsync(IEvent @event, CancellationToken token = default)
    {
        var message = JsonSerializer.Serialize(@event);
        
        await queueClient.SendMessageAsync(message, token);
    }

    public async Task<IEvent?> GetEventAsync(CancellationToken token = default)
    {
        throw new NotImplementedException(); 

        // var message = await queueClient.ReceiveMessageAsync(cancellationToken: token);
        //
        // if (!message.HasValue)
        // {
        //     return null;
        // }
        //
        // return JsonSerializer.Deserialize<IEvent>(message, );
    }
}