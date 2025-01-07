namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer;

using AssociationRegistry.Resources;

public class InvalidBevestigingstokenProvided : BadHttpRequestException
{
    public InvalidBevestigingstokenProvided() : base(ExceptionMessages.InvalidBevestigingstokenProvided)
    {
    }
}
