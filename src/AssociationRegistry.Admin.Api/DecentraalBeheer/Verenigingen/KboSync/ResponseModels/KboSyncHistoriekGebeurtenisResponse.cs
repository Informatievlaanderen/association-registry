namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.KboSync.ResponseModels;

using System.Runtime.Serialization;

/// <summary>Een gebeurtenis van een vereniging</summary>
[DataContract]
public class KboSyncHistoriekGebeurtenisResponse
{
    /// <summary>Het KBO nummer waarop de gebeurtenis plaatsvond</summary>
    [DataMember(Name = "kbonummer")]
    public string Kbonummer { get; set; } = null!;

    /// <summary>De unieke identificatie code van deze vereniging</summary>
    [DataMember(Name = "vCode")]
    public string VCode { get; init; } = null!;

    /// <summary>De beschrijving van de gebeurtenis</summary>
    [DataMember(Name = "beschrijving")]
    public string Beschrijving { get; set; } = null!;

    /// <summary>Het tijdstip waarop de gebeurtenis plaatsvond</summary>
    [DataMember(Name = "tijdstip")]
    public string Tijdstip { get; set; } = null!;
}
