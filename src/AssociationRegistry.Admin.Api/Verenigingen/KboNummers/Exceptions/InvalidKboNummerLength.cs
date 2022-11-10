namespace AssociationRegistry.Admin.Api.Verenigingen.KboNummers.Exceptions;

public class InvalidKboNummerLength : InvalidKboNummer
{
    public InvalidKboNummerLength() : base("Kbo nummer moet 10 cijfers bevatten")
    {
    }
}
