using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventStore.Azure.Events.Transport;

public class TransportEnvelope
{
    public required string Type { get; set; }
    public required bool IsLarge { get; set; }
    public required string Body { get; set; }

    public static TransportEnvelope Create(object @object)
    {
        if (@object is null)
        {
            throw new TransportEnvelopeException("Object is null");
        }

        return new TransportEnvelope
        {
            Type = @object.GetType().AssemblyQualifiedName!,
            IsLarge = false,
            Body = JsonSerializer.Serialize(@object)
        };
    }
}

// Good opportunity to write source generation so we don't have to do this for all events, commands, and projections
// or maybe god context class... (yuk?)
[JsonSerializable(typeof(TransportEnvelope))]
public partial class TransportEnvelopeContext : JsonSerializerContext;