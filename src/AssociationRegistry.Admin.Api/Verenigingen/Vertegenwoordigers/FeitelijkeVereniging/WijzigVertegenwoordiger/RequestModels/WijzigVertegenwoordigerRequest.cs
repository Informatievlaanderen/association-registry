namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger.RequestModels;

using System.Runtime.Serialization;
using AssociationRegistry.Acties.WijzigVertegenwoordiger;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;

[DataContract]
public class WijzigVertegenwoordigerRequest
{
    /// <summary>De te wijzigen vertegenwoordiger</summary>
    [DataMember(Name = "vertegenwoordiger")]
    public TeWijzigenVertegenwoordiger Vertegenwoordiger { get; set; } = null!;

    public WijzigVertegenwoordigerCommand ToCommand(string vCode, int vertegenwoordigerId)
        => new(
            VCode: VCode.Create(vCode),
            new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
                VertegenwoordigerId: vertegenwoordigerId,
                Rol: Vertegenwoordiger.Rol,
                Roepnaam: Vertegenwoordiger.Roepnaam,
                Email: Vertegenwoordiger.Email is null ? null : Email.Create(Vertegenwoordiger.Email),
                Telefoon: Vertegenwoordiger.Telefoon is null ? null : TelefoonNummer.Create(Vertegenwoordiger.Telefoon),
                Mobiel: Vertegenwoordiger.Mobiel is null ? null : TelefoonNummer.Create(Vertegenwoordiger.Mobiel),
                SocialMedia: Vertegenwoordiger.SocialMedia is null ? null : SocialMedia.Create(Vertegenwoordiger.SocialMedia),
                IsPrimair: Vertegenwoordiger.IsPrimair
            ));


}
