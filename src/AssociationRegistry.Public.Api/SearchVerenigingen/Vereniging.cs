namespace AssociationRegistry.Public.Api.SearchVerenigingen;

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
    [property: DataMember(Name = "Hoofdlocatie")]
    Locatie Hoofdlocatie,
    [property: DataMember(Name = "Doelgroep")]
    string Doelgroep,
    [property: DataMember(Name = "Locaties")]
    ImmutableArray<Locatie> Locaties,
    [property: DataMember(Name = "Activiteiten")]
    ImmutableArray<Activiteit> Activiteiten);

[DataContract]
public record Locatie(
    [property: DataMember(Name = "Type")] string Type,
    [property: DataMember(Name = "AdresId")]
    string AdresId,
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
