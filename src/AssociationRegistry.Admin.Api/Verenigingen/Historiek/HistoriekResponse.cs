namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using System;
using System.Runtime.Serialization;

/// <summary>Alle gebeurtenissen van deze vereniging</summary>
[DataContract]
public class HistoriekResponse
{
    /// <summary>De unieke identificatie code van deze vereniging</summary>
    [DataMember(Name = "vCode")]
    public string VCode { get; init; } = null!;

    /// <summary>Alle gebeurtenissen van deze vereniging</summary>
    [DataMember(Name = "gebeurtenissen")]
    public HistoriekGebeurtenisResponse[] Gebeurtenissen { get; init; } = Array.Empty<HistoriekGebeurtenisResponse>();
}

/// <summary>Een gebeurtenis van een vereniging</summary>
[DataContract]
public class HistoriekGebeurtenisResponse
{
    /// <summary>De beschrijving de gebeurtenis</summary>
    [DataMember(Name = "beschrijving")]
    public string Beschrijving { get; set; } = null!;

    /// <summary>Het type de gebeurtenis</summary>
    [DataMember(Name = "gebeurtenis")]
    public string Gebeurtenis { get; set; } = null!;

    /// <summary>De relevante data die hoort bij de gebeurtenis</summary>
    [DataMember(Name = "data")]
    public object? Data { get; set; }

    /// <summary>Instantie die de vereniging aanmaakt</summary>
    [DataMember(Name = "initiator")]
    public string Initiator { get; set; } = null!;

    /// <summary>Het tijdstip waarop de gebeurtenis plaatsvond</summary>
    [DataMember(Name = "tijdstip")]
    public string Tijdstip { get; set; } = null!;
}

