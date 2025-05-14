namespace AssociationRegistry.Events;

public record VertegenwoordigerWerdVerwijderd(int VertegenwoordigerId, string Insz, string Voornaam, string Achternaam) : IEvent
{
}
