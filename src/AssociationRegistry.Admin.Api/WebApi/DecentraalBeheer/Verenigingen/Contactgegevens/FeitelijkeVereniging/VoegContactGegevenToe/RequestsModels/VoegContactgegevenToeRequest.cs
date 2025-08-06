namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;

using AssociationRegistry.Vereniging;
using Common;
using DecentraalBeheer.Acties.Contactgegevens.VoegContactgegevenToe;
using DecentraalBeheer.Vereniging;
using System.Runtime.Serialization;

[DataContract]
public class VoegContactgegevenToeRequest
{
    /// <summary>Het toe te voegen contactgegeven</summary>
    [DataMember(Name = "contactgegeven")]
    public ToeTeVoegenContactgegeven Contactgegeven { get; set; } = null!;

    public VoegContactgegevenToeCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            DecentraalBeheer.Vereniging.Contactgegeven.CreateFromInitiator(
                Contactgegeventype.Parse(Contactgegeven.Contactgegeventype),
                Contactgegeven.Waarde,
                Contactgegeven.Beschrijving,
                Contactgegeven.IsPrimair));
}
