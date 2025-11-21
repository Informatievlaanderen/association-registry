namespace AssociationRegistry.Events;

public record VertegenwoordigerWerdVerwijderd(int VertegenwoordigerId, string Insz, string Voornaam, string Achternaam) : IEvent;
public record VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens(Guid RefId, int VertegenwoordigerId) : IEvent;
