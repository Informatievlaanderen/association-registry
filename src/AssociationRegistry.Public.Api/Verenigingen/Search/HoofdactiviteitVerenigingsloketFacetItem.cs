namespace AssociationRegistry.Public.Api.Verenigingen.Search;

public class HoofdactiviteitVerenigingsloketFacetItem
{
    /// <summary>
    /// De code van de hoofdactiviteit
    /// </summary>
    public string Code { get; init; }= null!;
    /// <summary>
    /// Het aantal gevonden vereingingen met deze hoofdactiviteit
    /// </summary>
    public long Aantal { get; init; }
    /// <summary>
    /// De query die kan uitgevoerd worden om te filteren op deze waarde
    /// </summary>
    public string Query { get; init; }= null!;


}
