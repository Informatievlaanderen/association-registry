namespace AssociationRegistry.Events;

using Framework;

public record VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
    string VCode,
    string KboNummer,
    string Naam,
    string KorteNaam) : IEvent;
