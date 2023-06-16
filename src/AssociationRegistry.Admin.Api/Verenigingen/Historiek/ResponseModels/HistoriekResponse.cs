namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek.ResponseModels;

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
