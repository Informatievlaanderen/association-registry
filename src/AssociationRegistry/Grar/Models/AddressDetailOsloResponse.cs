namespace AssociationRegistry.Grar.Models;

using Newtonsoft.Json;
using System.Runtime.Serialization;

[DataContract(Name = "AdresDetail", Namespace = "")]
public class AddressDetailOsloResponse
{

    /// <summary>
    /// De linked-data context van het adres.
    /// </summary>
    [DataMember(Name = "@context", Order = 0)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Context { get; }

    /// <summary>
    /// Het linked-data type van het adres.
    /// </summary>
    [DataMember(Name = "@type", Order = 1)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Type => "Adres";

    /// <summary>
    /// De identificator van het adres.
    /// </summary>
    [DataMember(Name = "Identificator", Order = 2)]
    [JsonProperty(Required = Required.DisallowNull)]
    public AdresIdentificator Identificator { get; set; }

    /// <summary>
    /// De gemeente die deel uitmaakt van het adres.
    /// </summary>
    [DataMember(Name = "Gemeente", Order = 3)]
    [JsonProperty(Required = Required.DisallowNull)]
    public AdresDetailGemeente Gemeente { get; set; }

    /// <summary>
    /// De postinfo die deel uitmaakt van het adres.
    /// </summary>
    [DataMember(Name = "Postinfo", Order = 4, EmitDefaultValue = false)]
    [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public AdresDetailPostinfo Postinfo { get; set; }

    /// <summary>
    /// De straatnaam die deel uitmaakt van het adres.
    /// </summary>
    [DataMember(Name = "Straatnaam", Order = 5)]
    [JsonProperty(Required = Required.DisallowNull)]
    public AdresDetailStraatnaam Straatnaam { get; set; }

    /// <summary>
    /// Homoniem toevoeging aan de straatnaam.
    /// </summary>
    [DataMember(Name = "HomoniemToevoeging", Order = 6, EmitDefaultValue = false)]
    [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public HomoniemToevoeging HomoniemToevoeging { get; set; }

    /// <summary>
    /// Het huisnummer van het adres.
    /// </summary>
    [DataMember(Name = "Huisnummer", Order = 7)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Huisnummer { get; set; }

    /// <summary>
    /// Het huisnummer waaraan het busnummer is gekoppeld.
    /// </summary>
    [DataMember(Name = "HuisnummerObject", Order = 8, EmitDefaultValue = false)]
    [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public AdresDetailHuisnummerObject? HuisnummerObject { get; set; }

    /// <summary>
    /// Het busnummer van het adres.
    /// </summary>
    [DataMember(Name = "Busnummer", Order = 9, EmitDefaultValue = false)]
    [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Busnummer { get; set; }

    /// <summary>
    /// Adresvoorstelling in de eerste officiële taal van de gemeente.
    /// </summary>
    [DataMember(Name = "VolledigAdres", Order = 10)]
    [JsonProperty(Required = Required.DisallowNull)]
    public VolledigAdres VolledigAdres { get; set; }

    /// <summary>
    /// De fase in het leven van het adres.
    /// </summary>
    [DataMember(Name = "AdresStatus", Order = 12)]
    [JsonProperty(Required = Required.DisallowNull)]
    public AdresStatus AdresStatus { get; set; }

    /// <summary>
    /// False wanneer het bestaan van het adres niet geweten is ten tijde van administratieve procedures, maar pas na observatie op het terrein.
    /// </summary>
    [DataMember(Name = "OfficieelToegekend", Order = 13)]
    [JsonProperty(Required = Required.DisallowNull)]
    public bool OfficieelToegekend { get; set; }
}

/// <summary>
/// De gemeente die deel uitmaakt van het adres.
/// </summary>
[DataContract(Name = "Gemeente", Namespace = "")]
public class AdresDetailGemeente
{
    /// <summary>
    /// De objectidentificator van de gekoppelde gemeente.
    /// </summary>
    [DataMember(Name = "ObjectId", Order = 1)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string ObjectId { get; set; }

    /// <summary>
    /// De URL die de details van de meest recente versie van de gekoppelde gemeente weergeeft.
    /// </summary>
    [DataMember(Name = "Detail", Order = 2)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Detail { get; set; }

    /// <summary>
    /// De gemeentenaam in de eerste officiële taal van de gemeente.
    /// </summary>
    [DataMember(Name = "Gemeentenaam", Order = 3)]
    [JsonProperty(Required = Required.DisallowNull)]
    public Gemeentenaam Gemeentenaam { get; set; }
}

/// <summary>
/// De postinfo die deel uitmaakt van het adres.
/// </summary>
[DataContract(Name = "PostInfo", Namespace = "")]
public class AdresDetailPostinfo
{
    /// <summary>
    /// De objectidentificator van de gekoppelde postinfo.
    /// </summary>
    [DataMember(Name = "ObjectId", Order = 1)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string ObjectId { get; set; }

    /// <summary>
    /// De URL die de details van de meest recente versie van de gekoppelde postinfo weergeeft.
    /// </summary>
    [DataMember(Name = "Detail", Order = 2)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Detail { get; set; }
}


/// <summary>
/// De straatnaam die deel uitmaakt van het adres.
/// </summary>
[DataContract(Name = "Straatnaam", Namespace = "")]
public class AdresDetailStraatnaam
{
    /// <summary>
    /// De objectidentificator van de gekoppelde straatnaam.
    /// </summary>
    [DataMember(Name = "ObjectId", Order = 1)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string ObjectId { get; set; }

    /// <summary>
    /// De URL die de details van de meest recente versie van de gekoppelde straatnaam weergeeft.
    /// </summary>
    [DataMember(Name = "Detail", Order = 2)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Detail { get; set; }

    /// <summary>
    /// De straatnaam in de eerste officiële taal van de gemeente.
    /// </summary>
    [DataMember(Name = "Straatnaam", Order = 3)]
    [JsonProperty(Required = Required.DisallowNull)]
    public GivenDifferentStraatnaam GivenDifferentStraatnaam { get; set; }
}


/// <summary>
/// De homoniemtoevoeging in de eerste officiële taal van de gemeente.
/// </summary>
[DataContract(Name = "HomoniemToevoeging", Namespace = "")]
public class HomoniemToevoeging
{
    /// <summary>
    /// De geografische naam.
    /// </summary>
    [DataMember(Name = "GeografischeNaam")]
    [JsonProperty(Required = Required.DisallowNull)]
    public GeografischeNaam GeografischeNaam { get; set; }
}

/// <summary>
/// Het huisnummer waaraan het busnummer is gekoppeld.
/// </summary>
[DataContract(Name = "AdresDetailHuisnummerObject", Namespace = "")]
public class AdresDetailHuisnummerObject
{
    /// <summary>
    /// De objectidentificator van het huisnummer waaraan het busnummer is gekoppeld.
    /// </summary>
    [DataMember(Name = "ObjectId", Order = 1)]
    [JsonProperty(Required = Required.DisallowNull)]
    public int ObjectId { get; set; }

    /// <summary>
    /// De URL die de details van de meest recente versie van het huisnummer waaraan het busnummer is gekoppeld weergeeft.
    /// </summary>
    [DataMember(Name = "Detail", Order = 2)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Detail { get; set; }
}
