namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VertegenwoordigerWerdGewijzigd(
    int VertegenwoordigerId,
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
    public static VertegenwoordigerWerdGewijzigd With(Vertegenwoordiger vertegenwoordiger)
        => new(
            vertegenwoordiger.VertegenwoordigerId,
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
