namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Emails;
using AssociationRegistry.DecentraalBeheer.Vereniging.SocialMedias;
using AssociationRegistry.DecentraalBeheer.Vereniging.TelefoonNummers;

public record WijzigVertegenwoordigerCommand(
    VCode VCode,
    WijzigVertegenwoordigerCommand.CommandVertegenwoordiger Vertegenwoordiger)
{
    public record CommandVertegenwoordiger(
        int VertegenwoordigerId,
        string? Rol,
        string? Roepnaam,
        Email? Email,
        TelefoonNummer? Telefoon,
        TelefoonNummer? Mobiel,
        SocialMedia? SocialMedia,
        bool? IsPrimair);
}
