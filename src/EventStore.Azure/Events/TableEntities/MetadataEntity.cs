namespace EventStore.Azure.Events.TableEntities;

public class MetadataEntity : TableEntity
{
    public int LastEvent { get; set; }
}