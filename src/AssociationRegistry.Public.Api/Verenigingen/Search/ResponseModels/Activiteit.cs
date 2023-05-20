namespace AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Activiteit
{
    /// <summary>
    /// Het id van deze activiteit
    /// </summary>
    [DataMember(Name = "Id")]
    public int Id { get; init; }

    /// <summary>
    /// De categorie van deze activiteit
    /// </summary>
    [DataMember(Name = "Categorie")]
    public string Categorie { get; init; } = null!;
}
