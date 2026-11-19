namespace AssociationRegistry.DecentraalBeheer.Vereniging.Koepels;

public record Koepelstatus
{
    public static readonly Koepelstatus Koepel = new("Koepel");
    public static readonly Koepelstatus GeenKoepel = new("GeenKoepel");
    public static readonly Koepelstatus NietBepaald = new("NietBepaald");

    public static readonly Koepelstatus[] All = [Koepel, GeenKoepel, NietBepaald];

    private Koepelstatus(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public bool IsKoepel => this == Koepel;
    public bool IsGeenKoepel => this == GeenKoepel;

    // Case-insensitive like Contactgegeventype; Parse returns the canonical instance ('koepel' -> Koepel).
    public static Koepelstatus Parse(string value) => All.Single(status => Matches(status, value));

    public static bool IsValid(string? value) => All.Any(status => Matches(status, value));

    private static bool Matches(Koepelstatus status, string? value) =>
        string.Equals(status.Value, value, StringComparison.InvariantCultureIgnoreCase);

    public override string ToString() => Value;
}
