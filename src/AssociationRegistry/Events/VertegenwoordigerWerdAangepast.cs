namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VertegenwoordigerWerdAangepast(
    int VertegenwoordigerId,
    string? Rol,
    string? Roepnaam,
    string? Email,
    string? Telefoon,
    string? Mobiel,
    string? SocialMedia,
    bool? IsPrimair) : IEvent
{
    public static VertegenwoordigerWerdAangepast With(Vertegenwoordiger vertegenwoordiger)
        => new(
            vertegenwoordiger.VertegenwoordigerId,
            vertegenwoordiger.Rol,
            vertegenwoordiger.Roepnaam,
            vertegenwoordiger.Email.Waarde,
            vertegenwoordiger.Telefoon.Waarde,
            vertegenwoordiger.Mobiel.Waarde,
            vertegenwoordiger.SocialMedia.Waarde,
            vertegenwoordiger.IsPrimair
        );
}
