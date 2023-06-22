namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

public class Adres
{
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
