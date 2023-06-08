namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe;

using System.Runtime.Serialization;
using AssociationRegistry.Acties.VoegVertegenwoordigerToe;
using Common;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;

[DataContract]
public class VoegVertegenwoordigerToeRequest
{
    /// <summary>De toe te voegen vertegenwoordiger</summary>
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
                Voornaam.Create(Vertegenwoordiger.Voornaam),
                Achternaam.Create(Vertegenwoordiger.Achternaam),
                Email.Create(Vertegenwoordiger.Email),
                TelefoonNummer.Create(Vertegenwoordiger.Telefoon),
                TelefoonNummer.Create(Vertegenwoordiger.Mobiel),
                SocialMedia.Create(Vertegenwoordiger.SocialMedia)));
}
