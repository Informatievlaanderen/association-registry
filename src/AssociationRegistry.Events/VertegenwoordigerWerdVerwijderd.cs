namespace AssociationRegistry.Events;


using Vereniging;

public record VertegenwoordigerWerdVerwijderd(int VertegenwoordigerId, string Insz, string Voornaam, string Achternaam) : IEvent
{
}
