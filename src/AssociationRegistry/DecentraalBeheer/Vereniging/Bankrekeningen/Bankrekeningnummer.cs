namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

public record Bankrekeningnummer
{
    public int Id { get; set; }
    public IBanNummer Iban { get; set; }
    public string GebruiktVoor {get; set;}
    public string Titularis { get; set; }

    public static Bankrekeningnummer Create(int nextId, ToeTevoegenBankrekeningnummer bankrekeningnummer)
        => new()
        {
            Id = nextId,
            Iban = bankrekeningnummer.IBAN,
            GebruiktVoor = bankrekeningnummer.GebruiktVoor,
            Titularis = bankrekeningnummer.Titularis,
        };

    public static Bankrekeningnummer Hydrate(int id, string iban, string gebruiktVoor, string titularis)
        => new()
        {
            Id = id,
            Iban = IBanNummer.Hydrate(iban),
            GebruiktVoor = gebruiktVoor,
            Titularis = titularis,
        };
}
