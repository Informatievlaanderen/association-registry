namespace AssociationRegistry.OpenTelemetry.Metrics;

using System.Diagnostics.Metrics;

/// <summary>
///     Telt hoe vaak een endpoint wordt aangeroepen met VR-Api-Version header,
///     opgesplitst per api/endpoint, gevonden versie en bron (header / none).
/// </summary>
public static class ApiVersionMetrics
{
    public const string MeterName = "VR.Api.VersionUsage";
    private const string MeterVersion = "1.0.0";

    public static class Apis
    {
        public const string Admin = "admin";
        public const string Public = "public";
    }

    public static class Versions
    {
        public const string V1 = "v1";
        public const string V2 = "v2";
        public const string Empty = "empty";
        public const string Other = "other";
    }

    public static class Sources
    {
        public const string Header = "header";
        public const string None = "none";
    }

    private static readonly Meter Meter = new(MeterName, MeterVersion);

    public static readonly Counter<long> ApiVersionUsage = Meter.CreateCounter<long>(
        name: "vr_api_version_usage_total",
        description: "Aantal requests per api, endpoint, vr-api-version waarde en bron (header/none)."
    );

    public static void Record(string api, string endpoint, string? rawVersion, string source)
    {
        ApiVersionUsage.Add(
            1,
            new KeyValuePair<string, object?>("api", api),
            new KeyValuePair<string, object?>("endpoint", endpoint),
            new KeyValuePair<string, object?>("version", NormalizeVersion(rawVersion)),
            new KeyValuePair<string, object?>("source", source)
        );
    }

    private static string NormalizeVersion(string? raw) =>
        string.IsNullOrWhiteSpace(raw)
            ? Versions.Empty
            : raw.Trim().ToLowerInvariant() switch
            {
                "v1" => Versions.V1,
                "v2" => Versions.V2,
                _ => Versions.Other,
            };
}
