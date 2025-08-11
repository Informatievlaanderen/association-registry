namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Stop.RequestModels;

using AssociationRegistry.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.StopVereniging;
using DecentraalBeheer.Vereniging;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

[DataContract]
public class StopVerenigingRequest
{
    /// <summary>
    /// De datum waarop de vereniging gestopt werd.
    /// </summary>
    [DataMember]
    [Required]
    public DateOnly? Einddatum { get; set; }

    public StopVerenigingCommand ToCommand(string vCode) => new(VCode.Create(vCode), Datum.CreateOptional(Einddatum));
}
