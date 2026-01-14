namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

using Magda.Kbo;

public record Bankrekeningnummer
{
    public const int MaxLengthDoel = 128;
    public const int MaxLengthTitularis = 128;

    public int BankrekeningnummerId { get; set; }
    public IbanNummer Iban { get; set; }
    public string Doel {get; set;}
    public string Titularis { get; set; }

    public static Bankrekeningnummer Create(int nextId, ToeTevoegenBankrekeningnummer bankrekeningnummer)
        => new()
        {
            BankrekeningnummerId = nextId,
            Iban = bankrekeningnummer.Iban,
            Doel = bankrekeningnummer.Doel,
            Titularis = bankrekeningnummer.Titularis,
        };

    public static Bankrekeningnummer Hydrate(int id, string iban, string doel, string titularis)
        => new()
        {
            BankrekeningnummerId = id,
            Iban = IbanNummer.Hydrate(iban),
            Doel = doel,
            Titularis = titularis,
        };

    public static Bankrekeningnummer CreateFromKbo(BankrekeningnummerVolgensKbo bankrekeningnummer, int id)
        => new()
        {
            BankrekeningnummerId = id,
            Iban = IbanNummer.Hydrate(bankrekeningnummer.Iban),
            Doel = string.Empty,
            Titularis = string.Empty,
        };

    public bool WouldBeEquivalent(Bankrekeningnummer bankrekeningnummer)
        => this == bankrekeningnummer with { BankrekeningnummerId = BankrekeningnummerId };

}
