namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;

using System.Runtime.Serialization;

/// <summary>Een gebeurtenis van een vereniging</summary>
[DataContract]
public class HistoriekGebeurtenisResponse
{
    /// <summary>De beschrijving van de gebeurtenis</summary>
    [DataMember(Name = "beschrijving")]
    public string Beschrijving { get; set; } = null!;

    /// <summary>Het type van de gebeurtenis</summary>
    [DataMember(Name = "gebeurtenis")]
    public string Gebeurtenis { get; set; } = null!;

    /// <summary>De relevante data die hoort bij de gebeurtenis</summary>
    [DataMember(Name = "data")]
    public object? Data { get; set; }

    /// <summary>Instantie die de vereniging heeft geregistreerd of gewijzigd</summary>
    [DataMember(Name = "initiator")]
    public string Initiator { get; set; } = null!;

    /// <summary>Het tijdstip waarop de gebeurtenis plaatsvond</summary>
    [DataMember(Name = "tijdstip")]
    public string Tijdstip { get; set; } = null!;
}
