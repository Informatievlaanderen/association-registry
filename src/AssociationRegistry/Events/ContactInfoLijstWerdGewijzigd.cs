namespace AssociationRegistry.Events;

using CommonEventDataTypes;
using Framework;

public record ContactInfoLijstWerdGewijzigd(
    string VCode,
    ContactInfo[] Toevoegingen,
    ContactInfo[] Verwijderingen) : IEvent;
