namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VertegenwoordigerWerdVerwijderd(int VertegenwoordigerId, string Insz, string Voornaam, string Achternaam) : IEvent
{
    public static VertegenwoordigerWerdVerwijderd With(Vertegenwoordiger vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId, vertegenwoordiger.Insz, vertegenwoordiger.Voornaam, vertegenwoordiger.Achternaam);
}
