namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public record Vereniging(
    [property: DataMember(Name = "VCode")] string VCode,
    [property: DataMember(Name = "Naam")] string Naam,
    [property: DataMember(Name = "KorteNaam")]
    string KorteNaam,
    [property: DataMember(Name = "Hoofdactiviteiten")]
    ImmutableArray<Hoofdactiviteit> Hoofdactiviteiten,
    [property: DataMember(Name = "Doelgroep")]
    string Doelgroep,
    [property: DataMember(Name = "Locaties")]
    ImmutableArray<Locatie> Locaties,
    [property: DataMember(Name = "Activiteiten")]
    ImmutableArray<Activiteit> Activiteiten,
    [property: DataMember(Name = "Links")] VerenigingLinks Links);

[DataContract]
public record Locatie(
    [property: DataMember(Name = "Type")] string Type,
    [property: DataMember(Name = "Hoofdlocatie",EmitDefaultValue = false)]
    bool Hoofdlocatie,
    [property: DataMember(Name = "Adres")]
    string Adres,
    [property: DataMember(Name = "Naam")]
    string? Naam,
    [property: DataMember(Name = "Postcode")]
    string Postcode,
    [property: DataMember(Name = "Gemeente")]
    string Gemeente
);

[DataContract]
public record Activiteit(
    [property: DataMember(Name = "Id")] int Id,
    [property: DataMember(Name = "Categorie")]
    string Categorie);

[DataContract]
public record Hoofdactiviteit(
    [property: DataMember(Name = "Code")] string Code,
    [property: DataMember(Name = "Name")] string Naam);

[DataContract]
public record VerenigingLinks(
    [property: DataMember(Name = "Detail")]
    Uri Detail
);
