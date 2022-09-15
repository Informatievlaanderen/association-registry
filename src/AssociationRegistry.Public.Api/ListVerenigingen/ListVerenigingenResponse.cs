using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace AssociationRegistry.Public.Api.ListVerenigingen;

[DataContract]
public record ListVerenigingenResponse(
    [property: DataMember(Name = "@context")]
    ListVerenigingContext Context,
    [property: DataMember(Name = "Verenigingen")]
    ImmutableArray<ListVerenigingenQueryResult> Verenigingen,
    [property: DataMember(Name = "Metadata")]
    Metadata Metadata);

[DataContract]
public record ListVerenigingenQueryResult(
    [property: DataMember(Name = "Id")] string Id,
    [property: DataMember(Name = "Naam")] string Naam,
    [property: DataMember(Name = "KorteNaam")]
    string KorteNaam,
    [property: DataMember(Name = "Hoofdlocatie")]
    Locatie Hoofdlocatie,
    [property: DataMember(Name = "Activiteiten")]
    ImmutableArray<string> Activiteiten,
    [property: DataMember(Name = "Links")] ImmutableArray<Link> Links
)
{
    public static ListVerenigingenQueryResult FromVereniging(VerenigingListItem vereniging) =>
        new(
            vereniging.Id,
            vereniging.Naam,
            vereniging.KorteNaam,
            vereniging.Hoofdlocatie,
            vereniging.Activiteiten,
            ImmutableArray.Create(Link.VerenigingDetail(vereniging.Id))
        );
}

[DataContract]
public record Locatie(
    [property: DataMember(Name = "Postcode")]
    string Postcode,
    [property: DataMember(Name = "Gemeentenaam")]
    string Gemeentenaam);

[DataContract]
public record VerenigingListItem(
    [property: DataMember(Name = "Id")] string Id,
    [property: DataMember(Name = "Naam")] string Naam,
    [property: DataMember(Name = "KorteNaam")]
    string KorteNaam,
    [property: DataMember(Name = "Hoofdlocatie")]
    Locatie Hoofdlocatie,
    [property: DataMember(Name = "Activiteiten")]
    ImmutableArray<string> Activiteiten);

