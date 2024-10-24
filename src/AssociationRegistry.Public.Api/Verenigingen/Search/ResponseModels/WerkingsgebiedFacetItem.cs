namespace AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class WerkingsgebiedFacetItem
{
    /// <summary>
    /// De code van het werkingsgebied
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>
    /// Het aantal gevonden verenigingen met dit werkingsgebied
    /// </summary>
    [DataMember(Name = "Aantal")]
    public long Aantal { get; set; }

    /// <summary>
    /// De query die kan uitgevoerd worden om te filteren op deze waarde
    /// </summary>
    [DataMember(Name = "Query")]
    public string Query { get; init; } = null!;

    /// <summary>
    /// De subfacets
    /// </summary>
    [DataMember(Name = "Subfacets")]
    public SubWerkingsgebiedFacetItem[] SubFacets { get; set; } = null!;
}

[DataContract]
public class SubWerkingsgebiedFacetItem
{
    /// <summary>
    /// De code van de hoofdactiviteit
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>
    /// Het aantal gevonden verenigingen met deze hoofdactiviteit
    /// </summary>
    [DataMember(Name = "Aantal")]
    public long Aantal { get; init; }

    /// <summary>
    /// De query die kan uitgevoerd worden om te filteren op deze waarde
    /// </summary>
    [DataMember(Name = "Query")]
    public string Query { get; init; } = null!;
}
