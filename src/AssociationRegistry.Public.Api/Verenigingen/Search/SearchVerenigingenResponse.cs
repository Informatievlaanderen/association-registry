namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public class SearchVerenigingenResponse
{
    /// <summary>
    /// Dit is de lijst van verenigingen die het resultaat van de zoekopdracht zijn.
    /// </summary>
    [DataMember(Name = "Verenigingen")] public ImmutableArray<Vereniging> Verenigingen { get; set; }

    /// <summary>
    /// Facets bevatten extra contextuele informatie over de zoekresultaten wat kan gebruikt worden voor verdere verfijning van de zoekopdracht.
    /// </summary>
    [DataMember(Name = "Facets")] public Facets? Facets { get; set; }

    /// <summary>
    /// In deze metadata plaatsen we alle relevante metadata voor de zoekopdracht, de paginering informatie.
    /// </summary>
    [DataMember(Name = "Metadata")] public Metadata? Metadata { get; set; }
}

[DataContract]
public class Facets
{
    /// <summary>
    /// De hoofdactivititeiten van deze vereniging volgens het verenigingsloket.
    /// </summary>
    [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    public ImmutableArray<HoofdactiviteitVerenigingsloketFacetItem>? HoofdactiviteitenVerenigingsloket { get; set; }
}
