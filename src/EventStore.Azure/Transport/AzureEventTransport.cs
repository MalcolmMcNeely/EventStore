using System.Text.Json;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.Azure;

public class AzureEventTransport(AzureService azureService) : IEventTransport
{
    public async Task SendEventAsync(IEvent @event, CancellationToken token = default)
    {
        var queueClient = azureService.QueueServiceClient.GetQueueClient(QueueConstants.TransportQueueName);
        var message = JsonSerializer.Serialize(@event);
        
        await queueClient.SendMessageAsync(message, token);
    }

    public Task<IEvent?> GetEventAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}