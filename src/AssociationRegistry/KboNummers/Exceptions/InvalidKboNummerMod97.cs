namespace AssociationRegistry.KboNummers.Exceptions;

public class InvalidKboNummerMod97 : InvalidKboNummer
{
    public InvalidKboNummerMod97() : base("Incorrect Kbo nummer: foutieve checksum")
    {
    }
}
