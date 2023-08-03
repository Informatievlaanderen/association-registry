namespace AssociationRegistry.Framework;

using NodaTime;

public record CommandMetadata(string Initiator, Instant Tijdstip, Guid CorrelationId, long? ExpectedVersion = null);
