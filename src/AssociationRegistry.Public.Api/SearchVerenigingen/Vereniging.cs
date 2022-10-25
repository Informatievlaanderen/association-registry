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
    ImmutableArray<string> Hoofdactiviteiten,
    [property: DataMember(Name = "Hoofdlocatie")]
    string Hoofdlocatie,
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
    [property: DataMember(Name = "Adresvoorstelling")]
    string Adresvoorstelling,
    [property: DataMember(Name = "postcode")]
    string Postcode,
    [property: DataMember(Name = "gemeente")]
    string Gemeente
);

[DataContract]
public record Activiteit(
    [property: DataMember(Name = "Id")] int Id,
    [property: DataMember(Name = "Categorie")]
    string Categorie);
