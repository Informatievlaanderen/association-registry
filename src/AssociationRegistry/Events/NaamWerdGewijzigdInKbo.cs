namespace AssociationRegistry.Events;

using Framework;

public record NaamWerdGewijzigdInKbo(string Naam) : IEvent;
