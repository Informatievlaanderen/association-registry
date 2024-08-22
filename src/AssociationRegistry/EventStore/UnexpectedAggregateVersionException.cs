namespace AssociationRegistry.EventStore;

using Resources;

public class UnexpectedAggregateVersionException : Exception
{
    public UnexpectedAggregateVersionException() : base(ExceptionMessages.UnexpectedAggregateVersion)
    {
    }

    public UnexpectedAggregateVersionException(string message) : base(message)
    {
    }
}
