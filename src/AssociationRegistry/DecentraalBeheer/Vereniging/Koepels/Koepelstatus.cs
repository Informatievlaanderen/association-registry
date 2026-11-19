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

    public static Koepelstatus Parse(string value) => All.Single(status => status.Value == value);

    public static bool IsValid(string? value) => All.Any(status => status.Value == value);

    public override string ToString() => Value;
}
