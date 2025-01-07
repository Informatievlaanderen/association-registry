namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven.RequestModels;

using AssociationRegistry.Acties.WijzigContactgegeven;
using AssociationRegistry.Vereniging;
using System.Runtime.Serialization;

[DataContract]
public class WijzigContactgegevenRequest
{
    /// <summary>Het te wijzigen contactgegeven</summary>
    [DataMember(Name = "contactgegeven")]
    public TeWijzigenContactgegeven Contactgegeven { get; set; } = null!;

    public WijzigContactgegevenCommand ToCommand(string vCode, int contactgegevenId)
        => new(
            VCode.Create(vCode),
            new WijzigContactgegevenCommand.CommandContactgegeven(
                contactgegevenId,
                Contactgegeven.Waarde,
                Contactgegeven.Beschrijving,
                Contactgegeven.IsPrimair));
}
