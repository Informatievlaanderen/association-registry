namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.WijzigVertegenwoordiger;

using System.Runtime.Serialization;
using Acties.WijzigVertegenwoordiger;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;

[DataContract]
public class WijzigVertegenwoordigerRequest
{
    /// <summary>Instantie die de wijziging uitvoert</summary>
    [DataMember(Name = "initiator")]
    public string Initiator { get; set; } = null!;

    /// <summary>De toe te voegen vertegenwoordiger</summary>
    [DataMember(Name = "vertegenwoordiger")]
    public TeWijzigenVertegenwoordiger Vertegenwoordiger { get; set; } = null!;

    public WijzigVertegenwoordigerCommand ToCommand(string vCode, int veretegenwoordigerId)
        => new(
            VCode: VCode.Create(vCode),
            new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
                VertegenwoordigerId: veretegenwoordigerId,
                Rol: Vertegenwoordiger.Rol,
                Roepnaam: Vertegenwoordiger.Roepnaam,
                Email: Vertegenwoordiger.Email is null ? null : Email.Create(Vertegenwoordiger.Email),
                Telefoon: Vertegenwoordiger.Telefoon is null ? null : TelefoonNummer.Create(Vertegenwoordiger.Telefoon),
                Mobiel: Vertegenwoordiger.Mobiel is null ? null : TelefoonNummer.Create(Vertegenwoordiger.Mobiel),
                SocialMedia: Vertegenwoordiger.SocialMedia is null ? null : SocialMedia.Create(Vertegenwoordiger.SocialMedia),
                IsPrimair: Vertegenwoordiger.IsPrimair
            ));

    public class TeWijzigenVertegenwoordiger
    {
        /// <summary>Dit is de rol van de vertegenwoordiger binnen de vereniging</summary>
        [DataMember]
        public string? Rol { get; set; }

        /// <summary>Dit is de roepnaam van de vertegenwoordiger</summary>
        [DataMember]
        public string? Roepnaam { get; set; }

        /// <summary>
        ///     Dit duidt aan dat dit de unieke primaire contactpersoon is voor alle communicatie met overheidsinstanties
        /// </summary>
        [DataMember]
        public bool? IsPrimair { get; set; }

        /// <summary>Het emailadres van de vertegenwoordiger</summary>
        [DataMember]
        public string? Email { get; set; }

        /// <summary>Het telefoonnummer van de vertegenwoordiger</summary>
        [DataMember]
        public string? Telefoon { get; set; }

        /// <summary>Het mobiel nummer van de vertegenwoordiger</summary>
        [DataMember]
        public string? Mobiel { get; set; }

        /// <summary>Het socialmedia account van de vertegenwoordiger</summary>
        [DataMember]
        public string? SocialMedia { get; set; }
    }
}
