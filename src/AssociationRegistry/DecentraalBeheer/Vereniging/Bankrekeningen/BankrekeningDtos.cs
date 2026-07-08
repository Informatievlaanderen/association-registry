namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

public record ToeTevoegenBankrekeningnummer
{
    public IbanNummer Iban { get; set; }
    public string Doel { get; set; }
    public Titularissen Titularissen { get; set; }
}

public record TeWijzigenBankrekeningnummer
{
    public int BankrekeningnummerId { get; set; }
    public string? Doel { get; set; }
    public string[]? Titularissen { get; set; }
}
