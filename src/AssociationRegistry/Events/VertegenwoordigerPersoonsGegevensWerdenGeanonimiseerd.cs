namespace AssociationRegistry.Events;

using NodaTime;

public record VertegenwoordigerPersoonsGegevensWerdenGeanonimiseerd(
    string VCode,
    int VertegenwoordigerId,
    string Reden,
    Instant Vervaldag
) : IEvent;
