namespace EventStore.Azure.Events;

internal sealed class RowKey
{
    internal required string RowPrefix { get; set; }
    internal int RowNumber { get; set; }

    public override string ToString()
    {
        return $"{RowPrefix}{RowNumber:0000000000}";
    }

    public static RowKey ForEventStream(int rowNumber)
    {
        return new RowKey
        {
            RowPrefix = Defaults.Streams.EventStreamRowPrefix,
            RowNumber = rowNumber
        };
    }
    
    public static RowKey ForMetadata()
    {
        return new RowKey
        {
            RowPrefix = Defaults.Streams.MetadataRowPrefix,
            RowNumber = 0
        };
    }

    public static RowKey Create(string rowPrefix, int rowNumber)
    {
        return new RowKey
        {
            RowPrefix = rowPrefix,
            RowNumber = rowNumber
        };
    }
}