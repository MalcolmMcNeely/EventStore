using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventStore.EFCore.Postgres.Events.Transport;

public class Envelope
{
    public required string Type { get; set; }
    public required bool IsLarge { get; set; }
    public required string Body { get; set; }

    public static Envelope Create(object @object)
    {
        if (@object is null)
        {
            throw new EnvelopeException("Object is null");
        }

        return new Envelope
        {
            Type = @object.GetType().AssemblyQualifiedName!,
            IsLarge = false,
            Body = JsonSerializer.Serialize(@object)
        };
    }
}

// Good opportunity to write source generation so we don't have to do this for all events, commands, and projections
// or maybe god context class... (yuk?)
[JsonSerializable(typeof(Envelope))]
public partial class TransportEnvelopeContext : JsonSerializerContext;