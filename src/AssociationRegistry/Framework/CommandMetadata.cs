namespace AssociationRegistry.Framework;

public record CommandMetadata(string Initiator, NodaTime.Instant Tijdstip, long? ExpectedVersion = null, bool WithForce = false);
