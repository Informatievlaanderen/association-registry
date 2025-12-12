namespace AssociationRegistry.Events;

[Obsolete("These are the upcasted events, only use this in projections and State")]
public record VertegenwoordigerWerdVerwijderd(int VertegenwoordigerId, string Insz, string Voornaam, string Achternaam) : IEvent;
public record VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens(Guid RefId, int VertegenwoordigerId) : IEvent;
