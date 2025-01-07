namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger.RequestModels;

using AssociationRegistry.Acties.WijzigVertegenwoordiger;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Emails;
using AssociationRegistry.Vereniging.SocialMedias;
using AssociationRegistry.Vereniging.TelefoonNummers;
using System.Runtime.Serialization;

[DataContract]
public class WijzigVertegenwoordigerRequest
{
    /// <summary>De te wijzigen vertegenwoordiger</summary>
    [DataMember(Name = "vertegenwoordiger")]
    public TeWijzigenVertegenwoordiger Vertegenwoordiger { get; set; } = null!;

    public WijzigVertegenwoordigerCommand ToCommand(string vCode, int vertegenwoordigerId)
        => new(
            VCode.Create(vCode),
            new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
                vertegenwoordigerId,
                Vertegenwoordiger.Rol,
                Vertegenwoordiger.Roepnaam,
                Vertegenwoordiger.Email is null ? null : Email.Create(Vertegenwoordiger.Email),
                Vertegenwoordiger.Telefoon is null ? null : TelefoonNummer.Create(Vertegenwoordiger.Telefoon),
                Vertegenwoordiger.Mobiel is null ? null : TelefoonNummer.Create(Vertegenwoordiger.Mobiel),
                Vertegenwoordiger.SocialMedia is null ? null : SocialMedia.Create(Vertegenwoordiger.SocialMedia),
                Vertegenwoordiger.IsPrimair
            ));
}
