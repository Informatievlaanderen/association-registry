namespace AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;

using System.ComponentModel;
using System.Runtime.Serialization;

[DataContract]
public class Vereniging
{
    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; set; }

    /// <summary>De vCode van de vereniging</summary>
    [DataMember(Name = "VCode")]
    public string VCode { get; init; } = null!;

    /// <summary>Het type van de vereniging</summary>
    [DataMember(Name = "Verenigingstype")]
    public VerenigingsType Verenigingstype { get; init; } = null!;

    /// <summary>De naam van de vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;

    /// <summary>Roepnaam van de vereniging. Dit veld is enkel aanwezig bij verenigingen met rechtspersoonlijkheid</summary>
    [DataMember(Name = "Roepnaam", EmitDefaultValue = false)]
    [DefaultValue(null)]
    public string? Roepnaam { get; set; }

    /// <summary>De korte naam van de vereniging</summary>
    [DataMember(Name = "KorteNaam")]
    public string KorteNaam { get; init; } = null!;

    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember(Name = "KorteBeschrijving")]
    public string KorteBeschrijving { get; init; } = null!;

    /// <summary>De lijst van hoofdactiviteiten erkend door het vereningingsloket</summary>
    [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; } = null!;

    /// <summary>De lijst van werkingsgebieden</summary>
    [DataMember(Name = "Werkingsgebieden")]
    public Werkingsgebied[] Werkingsgebieden { get; init; } = null!;

    /// <summary>De doelgroep waar de activiteiten van deze vereniging zich op concentreert</summary>
    [DataMember(Name = "Doelgroep")]
    public DoelgroepResponse Doelgroep { get; init; } = null!;

    /// <summary>De locaties waar de vereniging actief is</summary>
    [DataMember(Name = "Locaties")]
    public Locatie[] Locaties { get; init; } = null!;

    /// <summary>De sleutels van deze vereniging</summary>
    [DataMember(Name = "Sleutels")]
    public Sleutel[] Sleutels { get; init; } = null!;

    /// <summary>De relaties van deze vereniging</summary>
    [DataMember(Name = "Relaties")]
    public Relatie[] Relaties { get; init; } = null!;

    /// <summary>Weblinks i.v.m. deze vereniging</summary>
    [DataMember(Name = "Links")]
    public VerenigingLinks Links { get; init; } = null!;
}
