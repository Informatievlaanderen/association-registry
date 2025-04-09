namespace AssociationRegistry.Grar.Clients.Contracts;

using Newtonsoft.Json;
using System.Runtime.Serialization;

[DataContract(Name = "PostinfoDetail", Namespace = "")]
public class PostalInformationOsloResponse
{
    [DataMember(Name = "@context", Order = 0)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Context { get; set; }

    [DataMember(Name = "@type", Order = 1)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Type => "PostInfo";

    [DataMember(Name = "Identificator", Order = 2)]
    [JsonProperty(Required = Required.DisallowNull)]
    public PostinfoIdentificator Identificator { get; set; }

    [DataMember(Name = "Gemeente", Order = 3, EmitDefaultValue = false)]
    [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public PostinfoDetailGemeente? Gemeente { get; set; }

    [DataMember(Name = "Postnamen", Order = 4)]
    [JsonProperty(Required = Required.DisallowNull)]
    public List<Postnaam> Postnamen { get; set; }

    [DataMember(Name = "PostInfoStatus", Order = 5)]
    [JsonProperty(Required = Required.DisallowNull)]
    public PostInfoStatus PostInfoStatus { get; set; }

    [DataMember(Name = "Nuts3", Order = 5, EmitDefaultValue = false)]
    [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Nuts3Code { get; set; }
}

[DataContract(Name = "Identificator", Namespace = "")]
public class PostinfoIdentificator : Identificator
{
}

[DataContract(Name = "Gemeente", Namespace = "")]
public class PostinfoDetailGemeente
{
    [DataMember(Name = "ObjectId", Order = 1)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string ObjectId { get; set; }

    [DataMember(Name = "Detail", Order = 2)]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Detail { get; set; }

    [DataMember(Name = "Gemeentenaam", Order = 3)]
    [JsonProperty(Required = Required.DisallowNull)]
    public Gemeentenaam Gemeentenaam { get; set; }
}

[DataContract(Name = "Postnaam", Namespace = "")]
public class Postnaam
{
    [DataMember(Name = "GeografischeNaam")]
    [JsonProperty(Required = Required.DisallowNull)]
    public GeografischeNaam GeografischeNaam { get; set; }

    public Postnaam(GeografischeNaam geografischeNaam)
    {
        GeografischeNaam = geografischeNaam;
    }
}

[DataContract(Name = "PostInfoStatus", Namespace = "")]
public enum PostInfoStatus
{
    [EnumMember]
    Gerealiseerd = 1,

    [EnumMember]
    Gehistoreerd = 2,
}
