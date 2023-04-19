namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;

using System.Runtime.Serialization;
using Acties.VoegContactgegevenToe;
using Common;
using Vereniging;

[DataContract]
public class VoegContactgegevenToeRequest
{
    /// <summary>Instantie die de wijziging uitvoert</summary>
    [DataMember(Name = "initiator")]
    public string Initiator { get; set; } = null!;

    /// <summary>Het toe te voegen contactgegeven</summary>
    [DataMember(Name = "contactgegeven")]
    public ToeTeVoegenContactgegeven Contactgegeven { get; set; } = null!;

    public VoegContactgegevenToeCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            AssociationRegistry.Vereniging.Contactgegeven.Create(
                ContactgegevenType.Parse(Contactgegeven.Type),
                Contactgegeven.Waarde,
                Contactgegeven.Beschrijving,
                Contactgegeven.IsPrimair));
}
