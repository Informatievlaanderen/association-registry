namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer;

using AssociationRegistry.Resources;

public class InvalidBevestigingstokenProvided : BadHttpRequestException
{
    public InvalidBevestigingstokenProvided() : base(ExceptionMessages.InvalidBevestigingstokenProvided)
    {
    }
}
