namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

/// <summary>
/// Een uniek identificerende sleutel van deze vereniging in een externe bron
/// </summary>
[DataContract]
public class Sleutel
{
    /// <summary>
    /// De bron van de sleutel
    /// </summary>
    [DataMember(Name = "Bron")]
    public string Bron { get; set; } = null!;

    /// <summary>
    /// De externe identificator van de vereniging in de bron
    /// </summary>
    [DataMember(Name = "Waarde")]
    public string Waarde { get; set; } = null!;
}
