namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;

using System.Runtime.Serialization;
using AssociationRegistry.Acties.VoegContactgegevenToe;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Vereniging;

[DataContract]
public class VoegContactgegevenToeRequest
{
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
