namespace AssociationRegistry.Test.Common.Framework;

using AssociationRegistry.Framework;
using NodaTime;

public record TestCommandMetadata(string Initiator, Instant Tijdstip, Guid CorrelationId, long? ExpectedVersion = null)
    : CommandMetadata(Initiator, Tijdstip, CorrelationId, ExpectedVersion)
{
    public static TestCommandMetadata Empty => new(string.Empty, new Instant(), Guid.NewGuid(), null);
}

