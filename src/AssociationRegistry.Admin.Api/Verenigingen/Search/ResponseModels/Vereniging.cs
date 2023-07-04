namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Vereniging
{
    /// <summary>De vCode van de vereniging</summary>
    [DataMember(Name = "VCode")]
    public string VCode { get; init; } = null!;

    /// <summary>Het type van de vereniging</summary>
    [DataMember(Name = "Type")]
    public VerenigingsType Type { get; init; } = null!;

    /// <summary>De naam van de vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;

    /// <summary>De korte naam van de vereniging</summary>
    [DataMember(Name = "KorteNaam")]
    public string KorteNaam { get; init; } = null!;

    /// <summary>De lijst van hoofdactiviteiten erkend door het vereningingsloket</summary>
    [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; } = null!;

    /// <summary>De locaties waar de vereniging actief is</summary>
    [DataMember(Name = "Locaties")]
    public Locatie[] Locaties { get; init; } = null!;

    /// <summary>De sleutels van deze vereniging</summary>
    [DataMember(Name = "Sleutels")]
    public Sleutel[] Sleutels { get; init; } = null!;

    /// <summary>Weblinks i.v.m. deze vereniging</summary>
    [DataMember(Name = "Links")]
    public VerenigingLinks Links { get; init; } = null!;
}
