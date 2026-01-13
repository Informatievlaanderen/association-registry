namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

using Magda.Kbo;

public record Bankrekeningnummer
{
    public int BankrekeningnummerId { get; set; }
    public IbanNummer Iban { get; set; }
    public string GebruiktVoor {get; set;}
    public string Titularis { get; set; }

    public static Bankrekeningnummer Create(int nextId, ToeTevoegenBankrekeningnummer bankrekeningnummer)
        => new()
        {
            BankrekeningnummerId = nextId,
            Iban = bankrekeningnummer.Iban,
            GebruiktVoor = bankrekeningnummer.GebruiktVoor,
            Titularis = bankrekeningnummer.Titularis,
        };

    public static Bankrekeningnummer Hydrate(int id, string iban, string gebruiktVoor, string titularis)
        => new()
        {
            BankrekeningnummerId = id,
            Iban = IbanNummer.Hydrate(iban),
            GebruiktVoor = gebruiktVoor,
            Titularis = titularis,
        };

    public static Bankrekeningnummer CreateFromKbo(BankrekeningnummerVolgensKbo bankrekeningnummer, int id)
        => new()
        {
            BankrekeningnummerId = id,
            Iban = IbanNummer.Hydrate(bankrekeningnummer.Iban),
            GebruiktVoor = string.Empty,
            Titularis = string.Empty,
        };

    public bool WouldBeEquivalent(Bankrekeningnummer bankrekeningnummer)
        => this == bankrekeningnummer with { BankrekeningnummerId = BankrekeningnummerId };

}
