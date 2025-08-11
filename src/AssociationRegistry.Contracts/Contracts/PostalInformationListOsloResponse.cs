namespace AssociationRegistry.Contracts.Contracts;

using Newtonsoft.Json;
using System.Runtime.Serialization;

[DataContract(Name = "PostinfoCollectie", Namespace = "")]
public class PostalInformationListOsloResponse
{
    /// <summary>
    /// De linked-data context van de postinfo.
    /// </summary>
    [DataMember(Name = "@context", Order = 0)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Context { get; set; }

    /// <summary>
    /// De verzameling van postcodes.
    /// </summary>
    [DataMember(Name = "PostInfoObjecten", Order = 1)]
    [JsonProperty(Required = Required.DisallowNull)]
    public List<PostalInformationListItemOsloResponse> PostInfoObjecten { get; set; }

    ///// <summary>
    ///// Het totaal aantal gemeenten die overeenkomen met de vraag.
    ///// </summary>
    //[DataMember(Name = "TotaalAantal", Order = 2)]
    //[JsonProperty(Required = Required.DisallowNull)]
    //public long TotaalAantal { get; set; }

    /// <summary>
    /// De URL voor het ophalen van de volgende verzameling.
    /// </summary>
    [DataMember(Name = "Volgende", Order = 3, EmitDefaultValue = false)]
    [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Uri Volgende { get; set; }
}

[DataContract(Name = "PostInfo", Namespace = "")]
public class PostalInformationListItemOsloResponse
{
    /// <summary>
    /// Het linked-data type van de postinfo.
    /// </summary>
    [DataMember(Name = "@type", Order = 0)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Type => "PostInfo";

    /// <summary>
    /// De identificator van de postcode.
    /// </summary>
    [DataMember(Name = "Identificator", Order = 1)]
    [JsonProperty(Required = Required.DisallowNull)]
    public PostinfoIdentificator Identificator { get; set; }

    /// <summary>
    ///  De URL die de details van de meest recente versie van de postinfo weergeeft.
    /// </summary>
    [DataMember(Name = "Detail", Order = 2)]
    [JsonProperty(Required = Required.DisallowNull)]
    public Uri Detail { get; set; }

    /// <summary>
    /// De huidige fase in de doorlooptijd van de postcode.
    /// </summary>
    [DataMember(Name = "PostInfoStatus", Order = 3)]
    [JsonProperty(Required = Required.DisallowNull)]
    public PostInfoStatus PostInfoStatus { get; set; }

    /// <summary>
    /// De namen van het gebied dat de postcode beslaat, in de taal afkomstig uit het bPost bestand.
    /// </summary>
    [DataMember(Name = "Postnamen", Order = 3)]
    [JsonProperty(Required = Required.DisallowNull)]
    public List<Postnaam> Postnamen { get; set; }
}
