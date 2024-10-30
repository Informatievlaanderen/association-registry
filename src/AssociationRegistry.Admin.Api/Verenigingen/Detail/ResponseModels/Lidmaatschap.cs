﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

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
    /// De unieke identificator van de vereniging waarvan deze vereniging lid werd
    /// </summary>
    [DataMember(Name = "AndereVereniging")]
    public string AndereVereniging { get; set; } = null!;

    /// <summary>
    /// De naam van de gerelateerde vereniging
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; set; } = null!;

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

    /// <summary>De datum waarop het lidmaatschap start</summary>
    [DataMember(Name = "Van")]
    public string Van { get; init; } = string.Empty;

    /// <summary>De datum waarop het lidmaatschap stopt</summary>
    [DataMember(Name = "Tot")]
    public string Tot { get; init; } = string.Empty;
}
