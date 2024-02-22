namespace AssociationRegistry.Events;

using Framework;

public record KorteNaamWerdGewijzigdInKbo(string? KorteNaam) : IEvent;
