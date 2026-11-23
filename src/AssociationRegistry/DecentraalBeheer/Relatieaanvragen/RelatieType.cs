namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Exceptions;

public record RelatieType
{
    public static readonly RelatieType Lidmaatschap = new(Labels.Lidmaatschap);
    public static readonly RelatieType Subvereniging = new(Labels.Subvereniging);

    public static readonly RelatieType[] All = [Lidmaatschap, Subvereniging];

    private RelatieType(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public bool IsSubvereniging => this == Subvereniging;

    /// <summary>
    ///     Case-insensitive like Koepelstatus; returns the canonical instance. An unknown value throws
    ///     <see cref="RelatieTypeIsOngeldig" />, so a request does not validate the value itself.
    /// </summary>
    public static RelatieType Parse(string value) =>
        All.SingleOrDefault(type => Matches(type, value)) ?? throw new RelatieTypeIsOngeldig();

    private static bool Matches(RelatieType type, string? value) =>
        string.Equals(type.Value, value, StringComparison.InvariantCultureIgnoreCase);

    public override string ToString() => Value;

    public static class Labels
    {
        public const string Lidmaatschap = "Lidmaatschap";
        public const string Subvereniging = "Subvereniging";
    }
}
