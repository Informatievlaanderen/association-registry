namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VerwijderLidmaatschap.RequestModels;

using System.Runtime.Serialization;

[DataContract]
public class VerwijderLidmaatschapRequest
{
    /// <summary>
    /// De unieke identificator van het lidmaatschap
    /// </summary>
    [DataMember]
    public int LidmaatschapId { get; set; }
}
