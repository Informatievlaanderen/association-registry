namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class HoofdactiviteitVerenigingsloketFacetItem
{
    /// <summary>
    /// De code van de hoofdactiviteit
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; }= null!;
    /// <summary>
    /// Het aantal gevonden vereingingen met deze hoofdactiviteit
    /// </summary>
    [DataMember(Name = "Aantal")]
    public long Aantal { get; init; }
    /// <summary>
    /// De query die kan uitgevoerd worden om te filteren op deze waarde
    /// </summary>
    [DataMember(Name = "Query")]
    public string Query { get; init; }= null!;


}
