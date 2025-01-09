namespace AssociationRegistry.Events;



public record NaamWerdGewijzigd(string VCode, string Naam) : IEvent;
