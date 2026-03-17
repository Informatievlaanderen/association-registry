namespace AssociationRegistry.Framework;

using EventMetadata;
using NodaTime;

public record CommandMetadata(
    string Initiator,
    Instant Tijdstip,
    Guid CorrelationId,
    long? ExpectedVersion = null,
    EventMetadataCollection? AdditionalMetadata = null)
{
    public static CommandMetadata ForDigitaalVlaanderenProcess =>
        new(WellknownOvoNumbers.DigitaalVlaanderenOvoNumber, SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid());
};
