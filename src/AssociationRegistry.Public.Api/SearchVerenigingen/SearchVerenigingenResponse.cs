namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public class SearchVerenigingenResponse
{
    [DataMember(Name = "Verenigingen")] public ImmutableArray<Vereniging> Verenigingen { get; set; }
    [DataMember(Name = "Facets")] public Facets? Facets { get; set; }
    [DataMember(Name = "Metadata")] public Metadata? Metadata { get; set; }
}

[DataContract]
public class Facets
{
    [DataMember(Name = "hoofdactiviteiten")]
    public IImmutableDictionary<string, long>? HoofdActiviteiten { get; set; }
}
