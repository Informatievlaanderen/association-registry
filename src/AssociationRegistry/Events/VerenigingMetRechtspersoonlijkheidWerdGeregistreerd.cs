namespace AssociationRegistry.Events;

using Framework;

public record VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
    string VCode,
    string KboNummer,
    string Rechtsvorm,
    string Naam,
    string KorteNaam,
    DateOnly? Startdatum) : IEvent;
