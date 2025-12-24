namespace AssociationRegistry.KboMutations.SyncLambda.Messaging.Parsers;

using AssociationRegistry.KboMutations.CloudEvents;
using CloudNative.CloudEvents;
using System.Text.Json;

internal class CloudEventMessageParser : IMessageParser
{
    private readonly CloudEvent _cloudEvent;
    private readonly JsonElement _dataElement;

    public CloudEventMessageParser(CloudEvent cloudEvent)
    {
        _cloudEvent = cloudEvent;
        var dataJson = JsonSerializer.Serialize(cloudEvent.Data);
        _dataElement = JsonDocument.Parse(dataJson).RootElement;
    }

    public MagdaEnvelope ToEnvelope()
    {
        var kbo = GetString("KboNummer");
        var insz = GetString("Insz");
        var overleden = GetBool("Overleden");
        var parentContext = _cloudEvent.ExtractTraceContext();
        var sourceFileName = _cloudEvent.GetSourceFileName();

        return MagdaEnvelopeFactory.Create(kbo, insz, overleden, parentContext, sourceFileName);
    }

    private string? GetString(string prop)
        => _dataElement.TryGetProperty(prop, out var el) && el.ValueKind == JsonValueKind.String
            ? el.GetString()
            : null;

    private bool? GetBool(string prop)
        => _dataElement.TryGetProperty(prop, out var el) &&
           (el.ValueKind == JsonValueKind.True || el.ValueKind == JsonValueKind.False)
            ? el.GetBoolean()
            : null;
}
