namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Erkenning : IJsonLd
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>
    /// De unieke identificatie code van deze erkenning binnen de vereniging
    /// </summary>
    [DataMember(Name = "ErkenningId")]
    public int ErkenningId { get; set; }

    /// <summary>
    /// Gegevens initiatoren die deze erkenning geregistreerd hebben
    /// </summary>
    [DataMember(Name = "GeregistreerdDoor")]
    public GegevensInitiatorErkenning GeregistreerdDoor { get; set; }

    /// <summary>
    /// Het ipdc product van deze erkenning
    /// </summary>
    [DataMember(Name = "IpdcProduct")]
    public IpdcProduct IpdcProduct { get; set; } = null!;

    /// <summary>
    /// Startdatum van de erkenning
    /// </summary>
    [DataMember(Name = "Startdatum")]
    public string? Startdatum { get; set; }

    /// <summary>
    /// Einddatum van de erkenning
    /// </summary>
    [DataMember(Name = "Einddatum")]
    public string? Einddatum { get; set; }

    /// <summary>
    /// Datum waarop de erkenning hernieuwd kan worden
    /// </summary>
    [DataMember(Name = "Hernieuwingsdatum")]
    public string? Hernieuwingsdatum { get; set; }

    /// <summary>
    /// URL voor het hernieuwen van de erkenning
    /// </summary>
    [DataMember(Name = "HernieuwingsUrl")]
    public string HernieuwingsUrl { get; set; } = null!;

    /// <summary>
    /// Huidige status van de erkenning
    /// </summary>
    [DataMember(Name = "redenSchorsing")]
    public string RedenSchorsing { get; set; } = null!;

    /// <summary>
    /// Huidige status van de erkenning
    /// </summary>
    [DataMember(Name = "status")]
    public string Status { get; set; } = null!;
}
