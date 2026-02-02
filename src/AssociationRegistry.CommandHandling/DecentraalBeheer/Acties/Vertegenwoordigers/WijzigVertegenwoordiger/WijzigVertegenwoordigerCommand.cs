namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;

using System.Text;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Emails;
using AssociationRegistry.DecentraalBeheer.Vereniging.SocialMedias;
using AssociationRegistry.DecentraalBeheer.Vereniging.TelefoonNummers;

public record WijzigVertegenwoordigerCommand(
    VCode VCode,
    WijzigVertegenwoordigerCommand.CommandVertegenwoordiger Vertegenwoordiger
)
{
    public record CommandVertegenwoordiger(
        int VertegenwoordigerId,
        string? Rol,
        string? Roepnaam,
        Email? Email,
        TelefoonNummer? Telefoon,
        TelefoonNummer? Mobiel,
        SocialMedia? SocialMedia,
        bool? IsPrimair
    )
    {
        protected virtual bool PrintMembers(StringBuilder builder)
        {
            builder.Append($"VertegenwoordigerId = {VertegenwoordigerId}, ");
            builder.Append($"IsPrimair = {IsPrimair}");
            return true;
        }
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        return true;
    }
}
