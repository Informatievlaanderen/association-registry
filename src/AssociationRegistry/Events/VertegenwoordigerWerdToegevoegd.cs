namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record VertegenwoordigerWerdToegevoegd(
    string VCode,
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

    public static VertegenwoordigerWerdToegevoegd With(Vertegenwoordiger vertegenwoordiger, string vCode)
        => new(
            vCode,
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

public record VertegenwoordigerWerdToegevoegdEncrypted(
    string VCode,
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
}
