namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.InStopzetting.RequestModels;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.StopVereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.InStopzetting.UpdateInStopzetting;

[DataContract]
public class InStopzettingRequest
{
    /// <summary>
    /// Geeft aan of de vereniging zich in stopzetting bevindt.
    /// </summary>
    [DataMember]
    [Required]
    public bool InStopzetting { get; set; }

    public UpdateInStopzettingCommand ToCommand(string vCode) => new(vCode, InStopzetting);
}
