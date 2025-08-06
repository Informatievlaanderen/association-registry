namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Emails;
using DecentraalBeheer.Vereniging.SocialMedias;
using DecentraalBeheer.Vereniging.TelefoonNummers;
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
