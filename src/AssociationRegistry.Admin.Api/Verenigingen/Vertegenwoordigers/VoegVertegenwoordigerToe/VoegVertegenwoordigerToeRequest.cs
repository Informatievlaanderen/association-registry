namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VoegVertegenwoordigerToe;

using System.Runtime.Serialization;
using Acties.VoegVertegenwoordigerToe;
using Common;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;

[DataContract]
public class VoegVertegenwoordigerToeRequest
{
    /// <summary>Instantie die de wijziging uitvoert</summary>
    [DataMember(Name = "initiator")]
    public string Initiator { get; set; } = null!;

    /// <summary>Het toe te voegen vertegenwoordiger</summary>
    [DataMember(Name = "vertegenwoordiger")]
    public ToeTeVoegenVertegenwoordiger Vertegenwoordiger { get; set; } = null!;

    public VoegVertegenwoordigerToeCommand ToCommand(string vCode)
        => new(
            VCode: VCode.Create(vCode),
            AssociationRegistry.Vereniging.Vertegenwoordiger.Create(
                Insz.Create(Vertegenwoordiger.Insz),
                Vertegenwoordiger.IsPrimair,
                Vertegenwoordiger.Roepnaam,
                Vertegenwoordiger.Rol,
                Email.Create(Vertegenwoordiger.Email),
                TelefoonNummer.Create(Vertegenwoordiger.Telefoon),
                TelefoonNummer.Create(Vertegenwoordiger.Mobiel),
                SocialMedia.Create(Vertegenwoordiger.SocialMedia)));
}
