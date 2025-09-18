namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer;

using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using DecentraalBeheer.Vereniging.DuplicaatDetectie;
using System.Runtime.Serialization;

/// <summary>Een locatie van een vereniging</summary>
[DataContract]
public class Locatie
{
    public Locatie(
        string locatietype,
        bool isPrimair,
        string adres,
        string? naam,
        string postcode,
        string gemeente)
    {
        Locatietype = locatietype;
        IsPrimair = isPrimair;
        Adres = adres;
        Naam = naam;
        Postcode = postcode;
        Gemeente = gemeente;
    }

    public static Locatie FromDuplicaatVereniging(DuplicaatVereniging.Types.Locatie locatie)
        => new(locatie.Locatietype, locatie.IsPrimair, locatie.Adres, locatie.Naam, locatie.Postcode, locatie.Gemeente);

    /// <summary>Het soort locatie dat beschreven wordt</summary>
    [DataMember(Name = "Locatietype")]
    public string Locatietype { get; init; }

    /// <summary>Duidt aan dat dit de primaire locatie is</summary>
    [DataMember(Name = "IsPrimair")]
    public bool IsPrimair { get; init; }

    /// <summary>Het samengestelde adres van de locatie</summary>
    [DataMember(Name = "Adresvoorstelling")]
    public string Adres { get; init; }

    /// <summary>Een beschrijvende naam voor de locatie</summary>
    [DataMember(Name = "Naam")]
    public string? Naam { get; init; }

    /// <summary>Het busnummer van de locatie</summary>
    [DataMember(Name = "Postcode")]
    public string Postcode { get; init; }

    /// <summary>De gemeente van de locatie</summary>
    [DataMember(Name = "Gemeente")]
    public string Gemeente { get; init; }
}
