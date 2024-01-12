namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger.RequestModels;

using Acties.WijzigVertegenwoordiger;
using System.Runtime.Serialization;
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
