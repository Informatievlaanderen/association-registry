namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Contactgegeven
{
    /// <summary>Het type contactgegeven</summary>
    [DataMember(Name = "contactgegeventype")]
    public string Contactgegeventype { get; init; } = null!;

    /// <summary>De waarde van het contactgegeven</summary>
    [DataMember(Name = "waarde")]
    public string Waarde { get; init; }= null!;

    /// <summary>
    /// Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
    /// </summary>
    [DataMember(Name = "beschrijving")]
    public string Beschrijving { get; init; }= null!;

    /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
    [DataMember(Name = "isPrimair")]
    public bool IsPrimair { get; init; }
}
