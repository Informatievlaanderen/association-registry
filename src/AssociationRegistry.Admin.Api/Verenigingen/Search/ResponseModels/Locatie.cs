namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Locatie
{
    /// <summary>
    /// Waarvoor deze locatie gebruikt wordt
    /// </summary>
    [DataMember(Name = "Locatietype")]
    public string Locatietype { get; init; } = null!;

    /// <summary>
    /// Is dit de hoofdlocatie van deze vereniging
    /// </summary>
    [DataMember(Name = "Hoofdlocatie")]
    public bool Hoofdlocatie { get; init; }

    /// <summary>
    /// Het volledige adres van de vereniging
    /// </summary>
    [DataMember(Name = "Adres")]
    public string Adres { get; init; } = null!;

    ///<summary>
    /// De naam van de locatie voor de vereniging
    /// </summary>
    [DataMember(Name = "Naam")]
    public string? Naam { get; init; }

    /// <summary>
    /// De postcode van de locatie
    /// </summary>
    [DataMember(Name = "Postcode")]
    public string Postcode { get; init; } = null!;

    /// <summary>
    /// De gemeente waarin de locatie ligt
    /// </summary>
    [DataMember(Name = "Gemeente")]
    public string Gemeente { get; init; } = null!;
}
