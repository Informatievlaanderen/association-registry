namespace AssociationRegistry.MartenDb.BankrekeningnummerPersoonsgegevens;

using DecentraalBeheer.Vereniging.Bankrekeningen;

public record BankrekeningnummerPersoonsgegevensByIbanFilter
{
    public IbanNummer Iban { get; }

    public BankrekeningnummerPersoonsgegevensByIbanFilter(IbanNummer iban)
    {
        Iban = iban;
    }
}
