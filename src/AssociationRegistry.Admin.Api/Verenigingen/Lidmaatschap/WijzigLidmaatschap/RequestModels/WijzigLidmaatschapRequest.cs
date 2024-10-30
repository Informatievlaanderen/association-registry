namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;

using System.Runtime.Serialization;

[DataContract]
public class WijzigLidmaatschapRequest
{
    /// <summary>
    /// De unieke identificator van het lidmaatschap
    /// </summary>
    [DataMember]
    public int LidmaatschapId { get; set; }

    /// <summary>
    /// De datum waarop de relatie actief wordt
    /// </summary>
    [DataMember]
    public DateOnly? Van { get; set; }

    /// <summary>
    /// De datum waarop de relatie niet meer actief wordt
    /// </summary>
    [DataMember]
    public DateOnly? Tot { get; set; }

    /// <summary>
    /// De externe identificatie voor het lidmaatschap
    /// </summary>
    [DataMember]
    public string Identificatie { get; set; }

    /// <summary>
    /// De externe beschrijving van het lidmaatschap
    /// </summary>
    [DataMember]
    public string Beschrijving { get; set; }
}
