namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using Events;

public record VertegenwoordigerWerdToegevoegdData
(
    int VertegenwoordigerId,
    bool IsPrimair,
    string Roepnaam,
    string Rol,
    string Voornaam,
    string Achternaam,
    string Email,
    string Telefoon,
    string Mobiel,
    string SocialMedia
)
{
    public static VertegenwoordigerWerdToegevoegdData Create(VertegenwoordigerWerdToegevoegd e)
        => new(
            e.VertegenwoordigerId,
            e.IsPrimair,
            e.Roepnaam,
            e.Rol,
            e.Voornaam,
            e.Achternaam,
            e.Email,
            e.Telefoon,
            e.Mobiel,
            e.SocialMedia);
};
