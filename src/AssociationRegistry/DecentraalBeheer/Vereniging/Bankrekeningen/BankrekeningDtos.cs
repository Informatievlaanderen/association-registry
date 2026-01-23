namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

public record ToeTevoegenBankrekeningnummer
{
    public IbanNummer Iban { get; set; }
    public string Doel { get; set; }
    public Titularis Titularis { get; set; }
}

public record TeWijzigenBankrekeningnummer
{
    public int BankrekeningnummerId { get; set; }
    public string? Doel { get; set; }
    public string? Titularis { get; set; }
}
