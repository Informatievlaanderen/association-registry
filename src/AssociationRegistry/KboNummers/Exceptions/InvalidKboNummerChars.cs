namespace AssociationRegistry.KboNummers.Exceptions;

public class InvalidKboNummerChars : InvalidKboNummer
{
    public InvalidKboNummerChars() : base("Foutieve tekens in Kbo nummer")
    {
    }
}
