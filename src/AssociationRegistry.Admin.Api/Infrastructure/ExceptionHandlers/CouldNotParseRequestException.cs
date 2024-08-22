namespace AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;

using Resources;

public class CouldNotParseRequestException : Exception
{
    public CouldNotParseRequestException() : base(ExceptionMessages.CouldNotParseRequestException)
    {
    }
}
