namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Framework;
using Vereniging;
using Vereniging.Bronnen;

public record VertegenwoordigerWerdToegevoegd(
    int VertegenwoordigerId,
    string Insz,
    bool IsPrimair,
    string Roepnaam,
    string Rol,
    string Voornaam,
    string Achternaam,
    string Email,
    string Telefoon,
    string Mobiel,
    string SocialMedia) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;

    public static VertegenwoordigerWerdToegevoegd With(Vertegenwoordiger vertegenwoordiger)
        => new(
            vertegenwoordiger.VertegenwoordigerId,
            vertegenwoordiger.Insz,
            vertegenwoordiger.IsPrimair,
            vertegenwoordiger.Roepnaam ?? string.Empty,
            vertegenwoordiger.Rol ?? string.Empty,
            vertegenwoordiger.Voornaam,
            vertegenwoordiger.Achternaam,
            vertegenwoordiger.Email.Waarde,
            vertegenwoordiger.Telefoon.Waarde,
            vertegenwoordiger.Mobiel.Waarde,
            vertegenwoordiger.SocialMedia.Waarde
        );
}
