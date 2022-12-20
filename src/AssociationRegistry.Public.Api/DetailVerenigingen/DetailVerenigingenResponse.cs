namespace AssociationRegistry.Public.Api.DetailVerenigingen;

using System;
using System.Runtime.Serialization;

[DataContract]
public record Locatie(
    [property: DataMember(Name = "Postcode")]
    string Postcode,
    [property: DataMember(Name = "Gemeentenaam")]
    string Gemeentenaam);

[DataContract]
public record DetailVerenigingResponseWithActualData(
    [property: DataMember(Name = "@context")]
    string Context,
    [property: DataMember(Name = "Vereniging")]
    VerenigingDetailWithActualData VerenigingDetail,
    [property: DataMember(Name = "Metadata")]
    Metadata Metadata);

[DataContract]
public record VerenigingDetailWithActualData(
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
    [property: DataMember(Name = "Contacten")]
    ContactInfo[] Contacten);

public record Metadata(DateOnly DatumLaatsteAanpassing);

[DataContract]
public record ContactInfo(
    [property: DataMember(Name = "Contactnaam")]
    string? Contactnaam,
    [property: DataMember(Name = "Email")]
    string? Email,
    [property: DataMember(Name = "Telefoon")]
    string? Telefoon,
    [property: DataMember(Name = "Website")]
    string? Website
);
