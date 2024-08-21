namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using Microsoft.AspNetCore.Http;
using Resources;

public class InvalidBevestigingstokenProvided : BadHttpRequestException
{
    public InvalidBevestigingstokenProvided() : base(ExceptionMessages.InvalidBevestigingstokenProvided)
    {
    }
}
