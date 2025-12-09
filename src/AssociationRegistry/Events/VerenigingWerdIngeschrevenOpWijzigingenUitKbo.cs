namespace AssociationRegistry.Events;



public record VerenigingWerdIngeschrevenOpWijzigingenUitKbo(
    string KboNummer) : IEvent;
