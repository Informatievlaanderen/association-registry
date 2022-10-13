namespace AssociationRegistry.Admin.Api.Verenigingen;

using Events;

public record VerenigingWerdGeregistreerd(
    string VCode,
    string Naam
) : IEvent;
