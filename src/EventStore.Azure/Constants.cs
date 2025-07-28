namespace EventStore.Azure;

public class AzureContants
{
    public const string AzuriteConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
}

public class BlobContainerConstants
{
    public const string LargeEventContainerName = "large-events";
    public const string AggregateRootContainerName = "aggregate-roots";
    public const string ProjectionContainerName = "projections";
}

public class QueueConstants
{
    public const string TransportQueueName = "transport";
}

public class TableConstants
{
    public const string EventTableName = "events";
    public const string EventCursorsTableName = "eventcursors";

    public const int TableCreatedResponseCode = 409;
}