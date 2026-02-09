namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

using Magda.Kbo;

public record Bankrekeningnummer
{
    public const int MaxLengthDoel = 128;
    public const int MaxLengthTitularis = 128;

    public int BankrekeningnummerId { get; set; }
    public IbanNummer Iban { get; set; }
    public string Doel { get; set; }
    public Titularis Titularis { get; set; }
    public bool Gevalideerd { get; set; }
    public BankrekeningnummerBron Bron { get; set; }

    public static Bankrekeningnummer Create(int nextId, ToeTevoegenBankrekeningnummer bankrekeningnummer) =>
        new()
        {
            BankrekeningnummerId = nextId,
            Iban = bankrekeningnummer.Iban,
            Doel = bankrekeningnummer.Doel,
            Titularis = bankrekeningnummer.Titularis,
            Bron = BankrekeningnummerBron.Gi,
        };

    public static Bankrekeningnummer Hydrate(
        int id,
        string iban,
        string doel,
        string titularis,
        BankrekeningnummerBron bron,
        bool gevalideerd = false
    ) =>
        new()
        {
            BankrekeningnummerId = id,
            Iban = IbanNummer.Hydrate(iban),
            Doel = doel,
            Titularis = Titularis.Hydrate(titularis),
            Bron = bron,
            Gevalideerd = gevalideerd,
        };

    public static Bankrekeningnummer CreateFromKbo(BankrekeningnummerVolgensKbo bankrekeningnummer, int id) =>
        new()
        {
            BankrekeningnummerId = id,
            Iban = IbanNummer.Create(bankrekeningnummer.Iban),
            Doel = string.Empty,
            Titularis = Titularis.Hydrate(string.Empty),
            Bron = BankrekeningnummerBron.Kbo,
        };

    private Bankrekeningnummer CreateForWijzigen(string? doel, string? titularis) =>
        this with
        {
            Doel = doel ?? Doel,
            Titularis = Titularis.Replace(titularis),
        };

    public bool WouldBeEquivalent(Bankrekeningnummer bankrekeningnummer) =>
        this == bankrekeningnummer with { BankrekeningnummerId = BankrekeningnummerId };

    public bool WouldBeEquivalent(
        TeWijzigenBankrekeningnummer teWijzigenBankrekeningnummer,
        out Bankrekeningnummer gewijzigdBankrekeningnummer
    )
    {
        gewijzigdBankrekeningnummer = CreateForWijzigen(
            teWijzigenBankrekeningnummer.Doel,
            teWijzigenBankrekeningnummer.Titularis
        );

        return this == gewijzigdBankrekeningnummer;
    }
}

public sealed class BankrekeningnummerBron
{
    public string Value { get; }

    private BankrekeningnummerBron(string value)
    {
        Value = value;
    }

    public static readonly BankrekeningnummerBron Kbo = new("Kbo");
    public static readonly BankrekeningnummerBron Gi = new("Gi");

    public override string ToString() => Value;
}
