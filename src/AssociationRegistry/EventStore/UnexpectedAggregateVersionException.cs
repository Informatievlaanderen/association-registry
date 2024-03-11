namespace AssociationRegistry.EventStore;

public class UnexpectedAggregateVersionException : Exception
{
    public UnexpectedAggregateVersionException() : base(ExceptionMessages.UnexpectedAggregateVersion)
    {
    }

    public UnexpectedAggregateVersionException(string message) : base(message)
    {
    }
}
