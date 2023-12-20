namespace AssociationRegistry.EventStore;

using Marten.Exceptions;

public record StreamActionResult(long? Sequence, long? Version)
{
    public static StreamActionResult Empty
        => new(Sequence: null, Version: null);

    public Exception Exception { get; init; }

    public bool HasException => Exception is not null;

    public static StreamActionResult WithException(Exception exception)
        => Empty with
        {
            Exception = exception
        };
}
