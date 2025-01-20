namespace AssociationRegistry.Admin.Api.Verenigingen.Verwijder.RequestModels;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

[DataContract]
public class VerwijderVerenigingRequest
{
    /// <summary>
    /// De reden waarom de vereniging verwijderd werd.
    /// </summary>
    [DataMember]
    [Required]
    public string Reden { get; set; }
}
