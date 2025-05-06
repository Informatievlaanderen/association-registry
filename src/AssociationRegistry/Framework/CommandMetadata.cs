namespace AssociationRegistry.Framework;

using NodaTime;

public record CommandMetadata(string Initiator, Instant Tijdstip, Guid CorrelationId, long? ExpectedVersion = null)
{
    public static CommandMetadata ForDigitaalVlaanderenProcess =
        new CommandMetadata(AssociationRegistry.EventStore.EventStore.DigitaalVlaanderenOvoNumber, SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid());
};
