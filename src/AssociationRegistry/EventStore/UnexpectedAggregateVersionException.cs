namespace AssociationRegistry.EventStore;

using Microsoft.AspNetCore.Http;

public class UnexpectedAggregateVersionException : Exception
{
    public int StatusCode => StatusCodes.Status412PreconditionFailed;

    public UnexpectedAggregateVersionException()
    {
    }

    public UnexpectedAggregateVersionException(string message) : base(message)
    {
    }
}
