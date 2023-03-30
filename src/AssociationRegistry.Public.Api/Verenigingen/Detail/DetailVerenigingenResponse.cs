namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public record Locatie(
    [property: DataMember(Name = "Locatietype")]
    string Locatietype,
    [property: DataMember(Name = "Hoofdlocatie", EmitDefaultValue = false)]
    bool Hoofdlocatie,
    [property: DataMember(Name = "Adres")] string Adres,
    [property: DataMember(Name = "Naam")] string? Naam,
    [property: DataMember(Name = "Straatnaam")]
    string Straatnaam,
    [property: DataMember(Name = "Huisnummer")]
    string Huisnummer,
    [property: DataMember(Name = "Busnummer")]
    string? Busnummer,
    [property: DataMember(Name = "Postcode")]
    string Postcode,
    [property: DataMember(Name = "Gemeente")]
    string Gemeente,
    [property: DataMember(Name = "Land")] string Land
);

[DataContract]
public record DetailVerenigingResponse(
    [property: DataMember(Name = "@context")]
    string Context,
    [property: DataMember(Name = "Vereniging")]
    VerenigingDetail VerenigingDetail,
    [property: DataMember(Name = "Metadata")]
    Metadata Metadata);

[DataContract]
public record VerenigingDetail(
    [property: DataMember(Name = "VCode")] string VCode,
    [property: DataMember(Name = "Naam")] string Naam,
    [property: DataMember(Name = "KorteNaam")]
    string? KorteNaam,
    [property: DataMember(Name = "KorteBeschrijving")]
    string? KorteBeschrijving,
    [property: DataMember(Name = "Startdatum")]
    DateOnly? Startdatum,
    [property: DataMember(Name = "KboNummer")]
    string? KboNummer,
    [property: DataMember(Name = "Status")]
    string Status,
    [property: DataMember(Name = "Contactgegevens")]
    Contactgegeven[] Contactgegevens,
    [property: DataMember(Name = "Locaties")]
    ImmutableArray<Locatie> Locaties,
    [property: DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    ImmutableArray<HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket);

public record Metadata(string DatumLaatsteAanpassing);

[DataContract]
public record Contactgegeven(
    [property: DataMember(Name = "type")] string Type,
    [property: DataMember(Name = "waarde")]
    string Waarde,
    [property: DataMember(Name = "omschrijving")]
    string Omschrijving,
    [property: DataMember(Name = "isPrimair")]
    bool IsPrimair
);

[DataContract]
public record HoofdactiviteitVerenigingsloket(
    [property: DataMember(Name = "Code")] string Code,
    [property: DataMember(Name = "Beschrijving")]
    string Beschrijving);
