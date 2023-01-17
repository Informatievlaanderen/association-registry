namespace AssociationRegistry.Events;

using Framework;

public record NaamWerdGewijzigd(string VCode, string Naam) : IEvent;
