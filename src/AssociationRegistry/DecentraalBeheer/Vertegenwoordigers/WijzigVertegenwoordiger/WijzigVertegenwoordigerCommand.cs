namespace AssociationRegistry.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Emails;
using AssociationRegistry.Vereniging.SocialMedias;
using AssociationRegistry.Vereniging.TelefoonNummers;

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
