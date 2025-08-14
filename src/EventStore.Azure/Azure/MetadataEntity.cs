namespace EventStore.Azure.Azure;

public class MetadataEntity : TableEntity
{
    public int LastEvent { get; set; }
}