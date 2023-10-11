namespace AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;

using System;

public class CouldNotParseRequestException : Exception
{
    public CouldNotParseRequestException() : base(ExceptionMessages.CouldNotParseRequestException)
    {
    }
}
