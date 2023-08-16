namespace AssociationRegistry.Admin.Api.Infrastructure;

using System;

public class CouldNotParseRequestException : Exception
{
    public CouldNotParseRequestException() : base(ExceptionMessages.CouldNotParseRequestException)
    {
    }
}
