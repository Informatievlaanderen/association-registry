namespace AssociationRegistry.Events;

[Obsolete("These are the upcasted events, you might be looking for <EventName>+ZonderPersoonsgegevens")]
public record VertegenwoordigerWerdVerwijderd(int VertegenwoordigerId, string Insz, string Voornaam, string Achternaam) : IEvent;
public record VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens(Guid RefId, int VertegenwoordigerId) : IEvent;
