namespace AssociationRegistry.Events;

public record VerenigingWerdGeregistreerd(
    string VCode,
    string Naam
) : IEvent;
