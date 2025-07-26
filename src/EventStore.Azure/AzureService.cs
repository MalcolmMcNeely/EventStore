using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace EventStore.Azure;

public class AzureService(string connectionString = AzureContants.AzuriteConnectionString)
{
    public TableServiceClient TableServiceClient { get; } = new(connectionString);
    public BlobServiceClient BlobServiceClient { get; } = new(connectionString);
    public QueueServiceClient QueueServiceClient { get; } = new(connectionString);
}