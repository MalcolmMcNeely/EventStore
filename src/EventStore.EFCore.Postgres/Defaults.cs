namespace EventStore.EFCore.Postgres;

public static class Defaults
{
    public static class Events
    {
        public const string LargeEventContainerName = "large-events";
        public const string EventStoreTable = "events";
    }

    public static class Cursors
    {
        public const string TableName = "eventcursors";
        public const string PartitionKey = nameof(Cursors);
        public const string AllStreamCursor = nameof(AllStreamCursor);
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