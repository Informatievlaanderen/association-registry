namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Erkenning
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; set; }

    /// <summary>
    /// De unieke identificatie code van deze erkenning binnen de vereniging
    /// </summary>
    [DataMember(Name = "ErkenningId")]
    public int ErkenningId { get; set; }

    [DataMember(Name = "VCode")]
    public string VCode { get; set; } = null!;

    [DataMember(Name = "GeregistreerdDoor")]
    public GegevensInitiator[] GeregistreerdDoor { get; set; } = [];

    [DataMember(Name = "IpdcProduct")]
    public IpdcProduct IpdcProduct { get; set; } = null!;

    [DataMember(Name = "Startdatum")]
    public DateOnly Startdatum { get; set; }

    [DataMember(Name = "Einddatum")]
    public DateOnly Einddatum { get; set; }

    [DataMember(Name = "Hernieuwingsdatum")]
    public DateOnly Hernieuwingsdatum { get; set; }

    [DataMember(Name = "HernieuwingsUrl")]
    public string HernieuwingsUrl { get; set; } = null!;

    [DataMember(Name = "Motivering")]
    public string Motivering { get; set; } = null!;

}

