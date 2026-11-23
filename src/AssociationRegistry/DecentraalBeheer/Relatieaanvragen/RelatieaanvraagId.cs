namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

/// <summary>A readable id like a vCode, from its own sequence: RA0000001.</summary>
public record RelatieaanvraagId
{
    public const string Prefix = "RA";

    private RelatieaanvraagId(long nummer)
    {
        Nummer = nummer;
    }

    public long Nummer { get; }

    public string Value => $"{Prefix}{Nummer:0000000}";

    public static RelatieaanvraagId Create(long nummer) => new(nummer);

    public static RelatieaanvraagId Hydrate(string value) => new(long.Parse(value[Prefix.Length..]));

    public override string ToString() => Value;
}
