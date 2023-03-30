namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using Microsoft.AspNetCore.Http;

public class InvalidBevestigingstokenProvided : BadHttpRequestException
{
    public InvalidBevestigingstokenProvided() : base("Het bevestigingstoken is niet geldig voor deze request.")
    {
    }
}
