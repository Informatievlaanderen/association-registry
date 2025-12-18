namespace AssociationRegistry.KboMutations.SyncLambda;

using System.Text.Json;

public enum MagdaMessageType
{
    SyncKbo,
    SyncKsz,
    Unknown,
}

public record MagdaEnvelope(
    MagdaMessageType Type,
    string? KboNummer,
    TeSynchroniserenInszMessage? InszMessage
);

public static class MagdaEnvelopeParser
{
    public static MagdaEnvelope Parse(string body)
    {
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        var kbo = GetString(root, "KboNummer");
        var insz = GetString(root, "Insz");
        var overleden = GetBool(root, "Overleden");

        if (!string.IsNullOrWhiteSpace(kbo) && string.IsNullOrWhiteSpace(insz))
        {
            return new(
                MagdaMessageType.SyncKbo,
                kbo,
                null
            );
        }

        if (!string.IsNullOrWhiteSpace(insz) && string.IsNullOrWhiteSpace(kbo))
        {
            return new(
                MagdaMessageType.SyncKsz,
                null,
                new TeSynchroniserenInszMessage(insz, overleden ?? false)
            );
        }

        return new(
            MagdaMessageType.Unknown,
            kbo,
            insz is null ? null : new TeSynchroniserenInszMessage(insz, overleden ?? false)
        );
    }

    private static bool? GetBool(JsonElement root, string prop)
        => root.TryGetProperty(prop, out var el) &&
           (el.ValueKind == JsonValueKind.True || el.ValueKind == JsonValueKind.False)
            ? el.GetBoolean()
            : null;

    private static string? GetString(JsonElement root, string prop)
        => root.TryGetProperty(prop, out var el) && el.ValueKind == JsonValueKind.String
            ? el.GetString()
            : null;
}

public record TeSynchroniserenInszMessage(string Insz, bool Overleden);

