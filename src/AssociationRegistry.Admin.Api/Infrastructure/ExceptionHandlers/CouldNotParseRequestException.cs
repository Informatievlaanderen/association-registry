namespace AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;

using Resources;
using System;

public class CouldNotParseRequestException : Exception
{
    public CouldNotParseRequestException() : base(ExceptionMessages.CouldNotParseRequestException)
    {
    }
}
