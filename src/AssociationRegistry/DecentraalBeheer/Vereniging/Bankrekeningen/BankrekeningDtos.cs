namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

public record ToeTevoegenBankrekeningnummer
{
    public IBanNummer IBAN { get; set; }
    public string GebruiktVoor { get; set; }
    public string Titularis { get; set; }
}
