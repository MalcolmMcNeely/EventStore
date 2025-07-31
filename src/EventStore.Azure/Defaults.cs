namespace EventStore.Azure;

public static class Defaults
{
    public static class Azure
    {
        public const string AzuriteConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
    }

    public static class Events
    {
        public const string LargeEventContainerName = "large-events";
        public const string EventStoreTable = "events";
    }

    public static class Cursors
    {
        public const string TableName = "eventcursors";
        public const string PartitionKey = nameof(Cursors);
    }

    public static class AggregateRoot
    {
        public const string ContainerName = "aggregate-roots";
        public const string AggregateRootPartitionPrefix = "aggregate-root-";
    }

    public static class Projections
    {
        public const string ContainerName = "projections";
        public const string ProjectionPartitionPrefix = "projection-";
    }
    
    public static class Transport
    {
        public const string QueueName = "transport";
    }
    
    public static class Tables
    {
        public const int TableCreatedResponseCode = 409;
    }
    
    public static class Streams
    {
        public const string AllStreamPartition = "$All";
        public const string EventStreamRowPrefix = "A";
        public const string MetadataRowPrefix = "$";
    }
}

