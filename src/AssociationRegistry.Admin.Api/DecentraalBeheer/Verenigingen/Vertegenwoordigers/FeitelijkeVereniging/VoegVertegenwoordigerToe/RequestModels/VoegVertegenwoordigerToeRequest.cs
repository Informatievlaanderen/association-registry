namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe.RequestModels;

using AssociationRegistry.Acties.VoegVertegenwoordigerToe;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Emails;
using AssociationRegistry.Vereniging.SocialMedias;
using AssociationRegistry.Vereniging.TelefoonNummers;
using Common;
using System.Runtime.Serialization;

[DataContract]
public class VoegVertegenwoordigerToeRequest
{
    /// <summary>De toe te voegen vertegenwoordiger</summary>
    [DataMember(Name = "vertegenwoordiger")]
    public ToeTeVoegenVertegenwoordiger Vertegenwoordiger { get; set; } = null!;

    public VoegVertegenwoordigerToeCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
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
