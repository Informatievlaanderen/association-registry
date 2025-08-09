namespace AssociationRegistry.DecentraalBeheer.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;

using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;

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
