namespace AssociationRegistry.Public.Api.ListVerenigingen;

using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public record ListVerenigingenResponse(
    [property: DataMember(Name = "@context")]
    string Context,
    [property: DataMember(Name = "Verenigingen")]
    ImmutableArray<ListVerenigingenResponseItem> Verenigingen,
    [property: DataMember(Name = "Metadata")]
    Metadata Metadata);

[DataContract]
public record ListVerenigingenResponseItem(
    [property: DataMember(Name = "Id")] string Id,
    [property: DataMember(Name = "Naam")] string Naam,
    [property: DataMember(Name = "KorteNaam")]
    string KorteNaam,
    [property: DataMember(Name = "Hoofdlocatie")]
    Locatie Hoofdlocatie,
    [property: DataMember(Name = "Activiteiten")]
    ImmutableArray<Activiteit> Activiteiten,
    [property: DataMember(Name = "Links")] ImmutableArray<Link> Links
)
{
    public static ListVerenigingenResponseItem FromVereniging(VerenigingListItem vereniging)
        => new(
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
    ImmutableArray<Activiteit> Activiteiten);

public record Activiteit(
    [property: DataMember(Name = "Type")] string Type,
    [property: DataMember(Name = "Beheerder")]
    Uri Beheerder);
