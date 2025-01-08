namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven.RequestModels;

using Acties.Contactgegevens.WijzigContactgegevenFromKbo;
using AssociationRegistry.Vereniging;
using System.Runtime.Serialization;

[DataContract]
public class WijzigContactgegevenRequest
{
    /// <summary>Het te wijzigen contactgegeven</summary>
    [DataMember(Name = "contactgegeven")]
    public TeWijzigenContactgegeven Contactgegeven { get; set; } = null!;

    public WijzigContactgegevenFromKboCommand ToCommand(string vCode, int contactgegevenId)
        => new(
            VCode.Create(vCode),
            new WijzigContactgegevenFromKboCommand.CommandContactgegeven(
                contactgegevenId,
                Contactgegeven.Beschrijving,
                Contactgegeven.IsPrimair));
}
