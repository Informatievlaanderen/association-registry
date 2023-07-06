﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class VerenigingDetail
{
    /// <summary>De unieke identificatie code van deze vereniging</summary>
    [DataMember(Name = "VCode")]
    public string VCode { get; init; } = null!;

    /// <summary>Het type van deze vereniging</summary>
    [DataMember(Name = "Type")]
    public VerenigingsType Type { get; init; } = null!;

    /// <summary>Naam van de vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;

    /// <summary>Korte naam van de vereniging</summary>
    [DataMember(Name = "KorteNaam")]
    public string? KorteNaam { get; init; }

    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember(Name = "KorteBeschrijving")]
    public string? KorteBeschrijving { get; init; }

    /// <summary>Datum waarop de vereniging gestart is</summary>
    [DataMember(Name = "Startdatum")]
    public string? Startdatum { get; init; }

    /// <summary>De doelgroep waar de activiteiten van deze afdeling zich op concentreert</summary>
    [DataMember(Name = "Doelgroep")]
    public DoelgroepResponse Doelgroep { get; init; } = null!;

    /// <summary>Status van de vereniging</summary>
    [DataMember(Name = "Status")]
    public string Status { get; init; } = null!;

    /// <summary>Is deze vereniging uitgeschreven uit de publieke datastroom</summary>
    [DataMember(Name = "IsUitgeschrevenUitPubliekeDatastroom")]
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; init; }

    /// <summary>De contactgegevens van deze vereniging</summary>
    [DataMember(Name = "Contactgegevens")]
    public Contactgegeven[] Contactgegevens { get; init; } = null!;

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    [DataMember(Name = "Locaties")]
    public Locatie[] Locaties { get; init; } = null!;

    /// <summary>Alle vertegenwoordigers van deze vereniging</summary>
    [DataMember(Name = "Vertegenwoordigers")]
    public Vertegenwoordiger[] Vertegenwoordigers { get; init; } = null!;

    /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
    [DataMember(Name = "hoofdactiviteitenVerenigingsloket")]
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; } = null!;

    /// <summary>De sleutels van deze vereniging</summary>
    [DataMember(Name = "Sleutels")]
    public Sleutel[] Sleutels { get; init; } = null!;

    /// <summary>De relaties van deze vereniging</summary>
    [DataMember(Name = "Relaties")]
    public Relatie[] Relaties { get; init; } = null!;
}
