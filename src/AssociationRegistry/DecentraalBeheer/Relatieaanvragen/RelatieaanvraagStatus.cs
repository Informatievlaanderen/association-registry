namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Exceptions;

/// <summary>
///     Only Aangevraagd counts as open; the other statuses close a relatieaanvraag. Vervallen is set by the job of the
///     scheduled host, the day after the vervaldatum.
/// </summary>
public record RelatieaanvraagStatus
{
    public static readonly RelatieaanvraagStatus Aangevraagd = new(Labels.Aangevraagd);
    public static readonly RelatieaanvraagStatus Bevestigd = new(Labels.Bevestigd);
    public static readonly RelatieaanvraagStatus Geweigerd = new(Labels.Geweigerd);
    public static readonly RelatieaanvraagStatus Ingetrokken = new(Labels.Ingetrokken);
    public static readonly RelatieaanvraagStatus Vervallen = new(Labels.Vervallen);

    public static readonly RelatieaanvraagStatus[] All = [Aangevraagd, Bevestigd, Geweigerd, Ingetrokken, Vervallen];

    private RelatieaanvraagStatus(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    ///     Case-insensitive; returns the canonical instance. An unknown value throws
    ///     <see cref="RelatieaanvraagStatusIsOngeldig" />.
    /// </summary>
    public static RelatieaanvraagStatus Parse(string value) =>
        All.SingleOrDefault(status => Matches(status, value)) ?? throw new RelatieaanvraagStatusIsOngeldig();

    private static bool Matches(RelatieaanvraagStatus status, string? value) =>
        string.Equals(status.Value, value, StringComparison.InvariantCultureIgnoreCase);

    public override string ToString() => Value;

    public static class Labels
    {
        public const string Aangevraagd = "Aangevraagd";
        public const string Bevestigd = "Bevestigd";
        public const string Geweigerd = "Geweigerd";
        public const string Ingetrokken = "Ingetrokken";
        public const string Vervallen = "Vervallen";
    }
}
