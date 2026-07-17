namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.InStopzetting.RequestModels;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.StopVereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging;

[DataContract]
public class InStopzettingRequest
{
    /// <summary>
    /// De datum waarop de vereniging gestopt werd.
    /// </summary>
    [DataMember]
    [Required]
    public bool InStopzetting { get; set; }

    //public StopVerenigingCommand ToCommand(string vCode) => new(VCode.Create(vCode), Datum.CreateOptional(Einddatum));
}
