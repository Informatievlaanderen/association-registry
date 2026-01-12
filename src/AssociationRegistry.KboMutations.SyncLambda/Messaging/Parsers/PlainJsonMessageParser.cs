namespace AssociationRegistry.KboMutations.SyncLambda.Messaging.Parsers;

using System.Text.Json;

internal class PlainJsonMessageParser : IMessageParser
{
    private readonly JsonDocument _document;

    public PlainJsonMessageParser(JsonDocument document)
    {
        _document = document;
    }

    public SyncEnvelope ToEnvelope()
    {
        var kbo = GetString("kboNummer");
        var insz = GetString("insz");
        var overleden = GetBool("overleden");
        var correlationId = GetGuid("correlationId") ?? Guid.NewGuid();

        return SyncEnvelopeFactory.Create(kbo, insz, overleden, null, null, correlationId);
    }

    private string? GetString(string prop)
        => _document.RootElement.TryGetProperty(prop, out var el) && el.ValueKind == JsonValueKind.String
            ? el.GetString()
            : null;

    private bool? GetBool(string prop)
        => _document.RootElement.TryGetProperty(prop, out var el) &&
           (el.ValueKind == JsonValueKind.True || el.ValueKind == JsonValueKind.False)
            ? el.GetBoolean()
            : null;

    private Guid? GetGuid(string prop)
        => _document.RootElement.TryGetProperty(prop, out var el) && el.ValueKind == JsonValueKind.String
            ? Guid.TryParse(el.GetString(), out var guid) ? guid : null
            : null;
}
