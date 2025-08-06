namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.DetailAll.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Verenigingstype
{
    /// <summary>
    ///     Code van het type vereniging
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; set; } = null!;

    /// <summary>
    ///     Beschrijving van het type vereniging
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; set; } = null!;
}
