namespace AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Facets
{
    /// <summary>
    /// De hoofdactivititeiten van deze vereniging volgens het verenigingsloket
    /// </summary>
    [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    public HoofdactiviteitVerenigingsloketFacetItem[] HoofdactiviteitenVerenigingsloket { get; set; } = null!;
}
