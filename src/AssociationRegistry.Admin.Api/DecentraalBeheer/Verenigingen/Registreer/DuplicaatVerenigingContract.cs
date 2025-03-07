namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System.Collections.Immutable;
using System.Runtime.Serialization;

/// <summary>Een mogelijke duplicaat van de te registreren vereniging</summary>
[DataContract]
public class DuplicaatVerenigingContract
{
    public DuplicaatVerenigingContract(
        string vCode,
        string naam,
        string korteNaam,
        Verenigingstype verenigingstype,
        ImmutableArray<HoofdactiviteitVerenigingsloket> hoofdactiviteitenVerenigingsloket,
        ImmutableArray<Locatie> locaties,
        VerenigingLinks links)
    {
        VCode = vCode;
        Naam = naam;
        KorteNaam = korteNaam;
        Verenigingstype = verenigingstype;
        HoofdactiviteitenVerenigingsloket = hoofdactiviteitenVerenigingsloket;
        Locaties = locaties;
        Links = links;
    }

    /// <summary>De unieke identificatie code van deze vereniging</summary>
    [DataMember(Name = "VCode")]
    public string VCode { get; init; }

    /// <summary>Naam van de vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; }

    /// <summary>Korte naam van de vereniging</summary>
    [DataMember(Name = "KorteNaam")]
    public string KorteNaam { get; init; }

    /// <summary>Type van de vereniging</summary>
    [DataMember(Name = "Verenigingstype")]
    public Verenigingstype Verenigingstype { get; init; }

    /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
    [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    public ImmutableArray<HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket { get; init; }

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    [DataMember(Name = "Locaties")]
    public ImmutableArray<Locatie> Locaties { get; init; }

    /// <summary>Weblinks i.v.m. deze vereniging</summary>
    [DataMember(Name = "Links")]
    public VerenigingLinks Links { get; init; }

    /// <summary>
    /// Het subtype van de vereniging
    /// </summary>
    [DataMember(Name = "Verenigingssubtype", EmitDefaultValue = false)]
    public Verenigingssubtype Verenigingssubtype { get; set; }}
