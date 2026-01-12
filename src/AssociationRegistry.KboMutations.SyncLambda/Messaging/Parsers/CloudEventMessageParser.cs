namespace AssociationRegistry.KboMutations.SyncLambda.Messaging.Parsers;

using CloudNative.CloudEvents;
using Contracts.CloudEvents;
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

    public SyncEnvelope ToEnvelope()
    {
        var kbo = GetString("kboNummer");
        var insz = GetString("insz");
        var overleden = GetBool("overleden");
        var parentContext = _cloudEvent.ExtractTraceContext();
        var sourceFileName = _cloudEvent.GetSourceFileName();
        var correlationId = GetGuid("correlationId") ?? Guid.NewGuid();

        return SyncEnvelopeFactory.Create(kbo, insz, overleden, parentContext, sourceFileName, correlationId);
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

    private Guid? GetGuid(string prop)
        => _dataElement.TryGetProperty(prop, out var el) && el.ValueKind == JsonValueKind.String
            ? Guid.TryParse(el.GetString(), out var guid) ? guid : null
            : null;
}
