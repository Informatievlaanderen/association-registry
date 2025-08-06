namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.DetailAll.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Adres : IJsonLd
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>De straat van de locatie</summary>
    [DataMember(Name = "Straatnaam")]
    public string Straatnaam { get; init; } = null!;

    /// <summary>Het huisnummer van de locatie</summary>
    [DataMember(Name = "Huisnummer")]
    public string Huisnummer { get; init; } = null!;

    /// <summary>Het busnummer van de locatie</summary>
    [DataMember(Name = "Busnummer")]
    public string? Busnummer { get; init; }

    /// <summary>De postcode van de locatie</summary>
    [DataMember(Name = "Postcode")]
    public string Postcode { get; init; } = null!;

    /// <summary>De gemeente van de locatie</summary>
    [DataMember(Name = "Gemeente")]
    public string Gemeente { get; init; } = null!;

    /// <summary>Het land van de locatie</summary>
    [DataMember(Name = "Land")]
    public string Land { get; init; } = null!;
}
