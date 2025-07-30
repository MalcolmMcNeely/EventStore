namespace EventStore.Azure.Transport.Events.TableEntities;

public class MetadataEntity : TableEntity
{
    public int LastEvent { get; set; }
}