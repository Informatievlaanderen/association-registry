namespace AssociationRegistry.KboMutations.SyncLambda.Messaging.Parsers;

using System.Text.Json;

internal class PlainJsonMessageParser : IMessageParser
{
    private readonly JsonDocument _document;

    public PlainJsonMessageParser(JsonDocument document)
    {
        _document = document;
    }

    public MagdaEnvelope ToEnvelope()
    {
        var kbo = GetString("KboNummer");
        var insz = GetString("Insz");
        var overleden = GetBool("Overleden");

        return MagdaEnvelopeFactory.Create(kbo, insz, overleden, null, null);
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
}
