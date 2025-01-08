namespace AssociationRegistry.Events;



public record KorteNaamWerdGewijzigd(string VCode, string KorteNaam) : IEvent;
