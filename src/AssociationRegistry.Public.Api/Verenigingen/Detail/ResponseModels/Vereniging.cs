namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using System;
using System.Runtime.Serialization;

[DataContract]
public class Vereniging
{
    /// <summary>De unieke identificatie code van deze vereniging</summary>
    [DataMember(Name = "VCode")]
    public string VCode { get; init; } = null!;

    /// <summary>Type van de vereniging</summary>
    [DataMember(Name = "Type")]
    public VerenigingsType Type { get; init; } = null!;

    /// <summary>Naam van de vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;

    /// <summary>Roepnaam van de vereniging</summary>
    [DataMember(Name = "Roepnaam", EmitDefaultValue = false)]
    public string? Roepnaam { get; init; }

    /// <summary>Korte naam van de vereniging</summary>
    [DataMember(Name = "KorteNaam")]
    public string? KorteNaam { get; init; }

    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember(Name = "KorteBeschrijving")]
    public string? KorteBeschrijving { get; init; }

    /// <summary>Datum waarop de vereniging gestart is. Deze datum mag niet later zijn dan vandaag</summary>
    [DataMember(Name = "Startdatum")]
    public DateOnly? Startdatum { get; init; }

    /// <summary>De doelgroep waar de activiteiten van deze afdeling zich op concentreert</summary>
    [DataMember(Name = "Doelgroep")]
    public DoelgroepResponse Doelgroep { get; init; } = null!;

    /// <summary>Status van de vereniging</summary>
    [DataMember(Name = "Status")]
    public string Status { get; init; } = null!;

    /// <summary>De contactgegevens van deze vereniging</summary>
    [DataMember(Name = "Contactgegevens")]
    public Contactgegeven[] Contactgegevens { get; init; } = Array.Empty<Contactgegeven>();

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    [DataMember(Name = "Locaties")]
    public Locatie[] Locaties { get; init; } = Array.Empty<Locatie>();

    /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
    [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; } = Array.Empty<HoofdactiviteitVerenigingsloket>();

    /// <summary>De sleutels die deze vereniging beheren</summary>
    [DataMember(Name = "Sleutels")]
    public Sleutel[] Sleutels { get; init; } = Array.Empty<Sleutel>();

    /// <summary>De relaties van deze vereniging</summary>
    [DataMember(Name = "Relaties")]
    public Relatie[] Relaties { get; init; } = Array.Empty<Relatie>();
}
