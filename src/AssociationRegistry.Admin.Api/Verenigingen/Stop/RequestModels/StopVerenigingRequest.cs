namespace AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;

using Acties.StopVereniging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Vereniging;

[DataContract]
public class StopVerenigingRequest
{
    [DataMember]
    [Required]
    public DateOnly? Einddatum { get; set; }

    public StopVerenigingCommand ToCommand(string vCode) => new(VCode.Create(vCode), Datum.Create(Einddatum));
}
