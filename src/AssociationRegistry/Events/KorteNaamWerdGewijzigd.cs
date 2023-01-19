namespace AssociationRegistry.Events;

using Framework;

public record KorteNaamWerdGewijzigd(string VCode, string KorteNaam) : IEvent;
