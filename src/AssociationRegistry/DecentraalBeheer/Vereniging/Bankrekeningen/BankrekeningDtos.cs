namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

public record ToeTevoegenBankrekeningnummer
{
    public IbanNummer Iban { get; set; }
    public string GebruiktVoor { get; set; }
    public string Titularis { get; set; }
}
