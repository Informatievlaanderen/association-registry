namespace AssociationRegistry.Admin.Api.Infrastructure;

using System;

public class CouldNotParseRequestException : Exception
{
    public CouldNotParseRequestException() : base("Request kon niet correct behandeld worden. Controleer het formaat en probeer het opnieuw.")
    {
    }
}
