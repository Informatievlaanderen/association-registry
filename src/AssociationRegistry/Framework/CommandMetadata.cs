namespace AssociationRegistry.Framework;

using NodaTime;

public record CommandMetadata(string Initiator, Instant Tijdstip, long? ExpectedVersion = null);
