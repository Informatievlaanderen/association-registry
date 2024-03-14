namespace AssociationRegistry.Events;

using Framework;

public record SynchronisatieMetKboWasSuccesvol(string KboNummer) : IEvent;
