namespace AssociationRegistry.KboMutations.SyncLambda;

using System.Text.Json;

public enum MagdaMessageType
{
    SyncKbo,
    SyncKsz,
    Unknown,
}

public sealed record MagdaEnvelope(
    MagdaMessageType Type,
    string? KboNummer,
    string? Insz);

public static class MagdaEnvelopeParser
{
    public static MagdaEnvelope Parse(string body)
    {
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        static string? GetString(JsonElement root, string prop)
            => root.TryGetProperty(prop, out var el) && el.ValueKind == JsonValueKind.String
                ? el.GetString()
                : null;

        var kbo = GetString(root, "KboNummer");
        var insz = GetString(root, "Insz");

        if (!string.IsNullOrWhiteSpace(kbo) && string.IsNullOrWhiteSpace(insz))
            return new(MagdaMessageType.SyncKbo, kbo, null);

        if (!string.IsNullOrWhiteSpace(insz) && string.IsNullOrWhiteSpace(kbo))
            return new(MagdaMessageType.SyncKsz, null, insz);

        return new(MagdaMessageType.Unknown, kbo, insz);
    }
}
