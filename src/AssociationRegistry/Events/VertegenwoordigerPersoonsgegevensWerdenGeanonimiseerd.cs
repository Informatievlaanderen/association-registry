namespace AssociationRegistry.Events;

using NodaTime;

public record VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(
    string VCode,
    int VertegenwoordigerId,
    string Reden,
    Instant Vervaldag
) : IEvent;
