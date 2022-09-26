﻿namespace AssociationRegistry.Public.Api.DetailVerenigingen;

using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public record DetailVerenigingResponse(
    [property: DataMember(Name = "@context")]
    DetailVerenigingContext Context,
    [property: DataMember(Name = "Vereniging")]
    VerenigingDetail VerenigingDetail);

[DataContract]
public record Locatie(
    [property: DataMember(Name = "Postcode")]
    string Postcode,
    [property: DataMember(Name = "Gemeentenaam")]
    string Gemeentenaam);

[DataContract]
public record VerenigingDetail(
    [property: DataMember(Name = "Id")] string Id,
    [property: DataMember(Name = "Naam")] string Naam,
    [property: DataMember(Name = "KorteNaam")]
    string KorteNaam,
    [property: DataMember(Name = "KorteOmschrijving")]
    string KorteOmschrijving,
    [property: DataMember(Name = "Rechtsvorm")]
    string Rechtsvorm,
    [property: DataMember(Name = "StartDatum")]
    DateOnly? StartDatum,
    [property: DataMember(Name = "EindDatum")]
    DateOnly? EindDatum,
    [property: DataMember(Name = "Hoofdlocatie")]
    Locatie Hoofdlocatie,
    [property: DataMember(Name = "ContactPersoon")]
    ContactPersoon? ContactPersoon,
    [property: DataMember(Name = "Locaties")]
    ImmutableArray<Locatie> Locaties,
    [property: DataMember(Name = "Activiteiten")]
    ImmutableArray<string> Activiteiten,
    [property: DataMember(Name = "ContactGegevens")]
    ImmutableArray<ContactGegeven> ContactGegevens,
    [property: DataMember(Name = "LaatstGewijzigd")]
    DateOnly LaatstGewijzigd
);

public record ContactPersoon(string Voornaam, string Achternaam, ImmutableArray<ContactGegeven> ContactGegevens);

public record ContactGegeven(string Type, string Waarde);
