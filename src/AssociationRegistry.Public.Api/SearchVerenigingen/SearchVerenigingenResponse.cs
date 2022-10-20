namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public record SearchVerenigingenResponse(
    [property: DataMember(Name = "Verenigingen")]
    ImmutableArray<Vereniging> Verenigingen,
    [property: DataMember(Name = "Facets")]
    ImmutableDictionary<string, long> Facets,
    [property: DataMember(Name = "Metadata")]
    Metadata Metadata);
