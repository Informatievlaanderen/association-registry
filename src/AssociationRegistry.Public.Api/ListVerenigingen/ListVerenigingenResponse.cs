using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace AssociationRegistry.Public.Api.ListVerenigingen;

[DataContract]
public record ListVerenigingenResponse(
    [property: DataMember(Name = "@context")] ListVerenigingContext Context,
    [property: DataMember(Name = "Verenigingen")] ImmutableArray<Vereniging> Verenigingen,
    [property: DataMember(Name = "Metadata")] Metadata Metadata);


[DataContract]
public record Vereniging(
    [property: DataMember(Name = "Id")] string Id,
    [property: DataMember(Name = "Naam")] string Naam,
    [property: DataMember(Name = "KorteNaam")] string KorteNaam,
    [property: DataMember(Name = "Locaties")] ImmutableArray<Locatie> Locaties,
    [property: DataMember(Name = "Activiteiten")] ImmutableArray<string> Activiteiten);

public record Locatie(
    string Postcode,
    string Gemeentenaam);
