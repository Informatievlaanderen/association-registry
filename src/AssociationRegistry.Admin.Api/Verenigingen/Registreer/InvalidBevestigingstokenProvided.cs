namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using Resources;

public class InvalidBevestigingstokenProvided : BadHttpRequestException
{
    public InvalidBevestigingstokenProvided() : base(ExceptionMessages.InvalidBevestigingstokenProvided)
    {
    }
}
