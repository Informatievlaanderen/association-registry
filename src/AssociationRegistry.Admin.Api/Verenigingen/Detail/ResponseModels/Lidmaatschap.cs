namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Lidmaatschap
{
    /// <summary>
    /// Het id van het lidmaatschap
    /// </summary>
    [DataMember(Name = "Lidmaatschap")]
    public int LidmaatschapId { get; set; }

    /// <summary>
    /// De unieke identificator van de andere vereniging in het verenigingsregister
    /// </summary>
    [DataMember(Name = "VCode")]
    public string AndereVereniging { get; set; } = null!;

    /// <summary>
    /// De naam van de gerelateerde vereniging
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; set; } = null!;

    // /// <summary>
    // /// De link naar het beheer detail van de gerelateerde vereniging
    // /// </summary>
    // [DataMember(Name = "Detail")]
    // public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// De identificatie van het lidmaatschap
    /// </summary>
    [DataMember(Name = "Identificatie")]
    public string Identificatie { get; set; } = string.Empty;

    /// <summary>
    /// De beschrijving van het lidmaatschap
    /// </summary>
    [DataMember(Name = "Beschrijving")]
    public string Beschrijving { get; set; } = string.Empty;

    /// <summary>Datum waarop het lidmaatschap gestart is</summary>
    [DataMember(Name = "DatumVanaf")]
    public string Van { get; init; } = string.Empty;

    /// <summary>Datum waarop het lidmaatschap gestopt is</summary>
    [DataMember(Name = "DatumTot")]
    public string Tot { get; init; } = string.Empty;
}
