using System.Text.Json;
using EventStore.Events;
using EventStore.Transport;

namespace EventStore.Azure;

public class AzureEventTransport(AzureService azureService) : IEventTransport
{
    public void SendEvent(IEvent @event)
    {
        var queueClient = azureService.QueueServiceClient.GetQueueClient(QueueConstants.TransportQueueName);
        var message = JsonSerializer.Serialize(@event);
        
        queueClient.SendMessageAsync(message);
    }
}