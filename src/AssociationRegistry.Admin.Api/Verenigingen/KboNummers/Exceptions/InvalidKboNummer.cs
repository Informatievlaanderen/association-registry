namespace AssociationRegistry.Admin.Api.Verenigingen.KboNummers.Exceptions;

using System;

public class InvalidKboNummer : Exception
{
    public InvalidKboNummer() : base("Kbo nummer is ongeldig")
    {
    }
}
